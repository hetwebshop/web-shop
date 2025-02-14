using API.Data;
using API.Data.IUserOfferRepository;
using API.DTOs;
using API.Entities.JobPost;
using API.Entities.Notification;
using API.Extensions;
using API.Helpers;
using API.Mappers;
using API.PaginationEntities;
using API.Services;
using API.Services.UserOfferServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using JobPostStatus = API.Helpers.JobPostStatus;

namespace API.Controllers
{
    [Authorize]
    public class JobController : BaseController
    {
        private readonly IUserJobPostService _jobPostService;
        private readonly IUnitOfWork _uow;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IUserJobPostRepository _userJobPostRepository;
        private readonly DataContext _dbContext;
        private readonly IEmailService emailService;
        private readonly string UIBaseUrl;
        private readonly IConfiguration _configuration;

        public JobController(IUserJobPostService jobPostService, IUnitOfWork uow, IBlobStorageService blobStorageService, IUserJobPostRepository userJobPostRepository, DataContext dbContext, IEmailService emailService, IConfiguration configuration)
        {
            _jobPostService = jobPostService;
            _uow = uow;
            _blobStorageService = blobStorageService;
            _userJobPostRepository = userJobPostRepository;
            _dbContext = dbContext;
            this.emailService = emailService;
            UIBaseUrl = configuration.GetSection("UIBaseUrl").Value;
            _configuration = configuration;
        }

        [HttpPost("ads")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAds([FromBody] AdsPaginationParameters adsParameters)
        {
            adsParameters.adStatus = JobPostStatus.Active;
            var jobPosts = await _jobPostService.GetJobPostsAsync(adsParameters);
            var pagedResponse = jobPosts.ToPagedResponse();
            return Ok(pagedResponse);
        }

        [HttpPost("ads-private")]
        public async Task<IActionResult> GetAdsPrivate([FromBody] AdsPaginationParameters adsParameters)
        {
            var userId = HttpContext.User.GetUserId();
            if (userId == null)
                return Unauthorized("Nemate pravo pristupa!");
            adsParameters.adStatus = JobPostStatus.Active;
            var jobPosts = await _jobPostService.GetJobPostsAsync(adsParameters);
            var pagedResponse = jobPosts.ToPagedResponse();
            var contactedAdsByCurrentUser = await _dbContext.ContactUserRequests.Where(r => r.FromUserId == userId).ToListAsync();
            var contactedAdsByCurrentUserIds = contactedAdsByCurrentUser.Select(r => r.Id);
            foreach(var item in pagedResponse.Items)
            {
                if (contactedAdsByCurrentUserIds.Contains(item.Id) || item.SubmittingUserId == userId)
                    item.CanCurrentUserApplyOnAd = false;
            }
            return Ok(pagedResponse);
        }

        [HttpPost("my-ads")]
        public async Task<IActionResult> GetMyAds([FromBody] AdsPaginationParameters adsParameters)
        {
            var currentUserId = HttpContext.User.GetUserId();
            adsParameters.UserId = currentUserId;
            var myAds = await _jobPostService.GetUserJobPostsAsync(adsParameters);
            var pagedResponse = myAds.ToPagedResponse();
            return Ok(pagedResponse);
        }

        [HttpGet("user-job/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserJobById(int id)
        {
            var userJob = await _jobPostService.GetUserJobPostByIdAsync(id);
            if(userJob.IsDeleted || userJob.JobPostStatusId != (int)JobPostStatus.Active || userJob.AdEndDate < DateTime.Now)
                return NotFound("Oglas je obrisan, zatvoren, ili je istekao.");
            return Ok(userJob);
        }

        [HttpGet("user-job-private/{id}")]
        public async Task<IActionResult> GetUserJobByIdPrivate(int id)
        {
            var userId = HttpContext.User.GetUserId();
            if (userId == null)
                return Unauthorized("Nemate pravo pristupa!");
            var userJob = await _jobPostService.GetUserJobPostByIdAsync(id);
            if (userJob.IsDeleted || userJob.JobPostStatusId != (int)JobPostStatus.Active || userJob.AdEndDate < DateTime.Now)
                return NotFound("Oglas je obrisan, zatvoren, ili je istekao.");
            var contactedAdsByCurrentUserIds = await _dbContext.ContactUserRequests.Where(r => r.FromUserId == userId).Select(r => r.FromUserId).ToListAsync();
            if (contactedAdsByCurrentUserIds.Contains(userJob.Id) || userJob.SubmittingUserId == userId)
                userJob.CanCurrentUserApplyOnAd = false;
            return Ok(userJob);
        }

        [HttpGet("my-ad/{id}")]
        public async Task<IActionResult> GetMyJobById(int id)
        {
            var currentUserId = HttpContext.User.GetUserId();
            var userJob = await _jobPostService.GetUserJobPostByIdAsync(id);
            var valid = await CheckDoesAdBelongsToUser(userJob, currentUserId);
            if(!valid)
                return Unauthorized("Nemate pravo pristupa ovom oglasu");
            return Ok(userJob);
        }

        private async Task<bool> CheckDoesAdBelongsToUser(UserJobPostDto userJob, int userId)
        {
            if (userJob != null && userJob.SubmittingUserId != userId)
                return false;
            return true;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUserJobPost([FromForm]UserJobPostDto userJobPostDto)
        {
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["applicantEducations"]))
                {
                    var applicantEducationsJson = Request.Form["applicantEducations"];
                    userJobPostDto.ApplicantEducations = JsonConvert.DeserializeObject<List<ApplicantEducationDto>>(applicantEducationsJson);
                }
                if (!string.IsNullOrEmpty(Request.Form["applicantPreviousCompanies"]))
                {
                    var applicantPrevCompaniesJson = Request.Form["applicantPreviousCompanies"];
                    userJobPostDto.ApplicantPreviousCompanies = JsonConvert.DeserializeObject<List<ApplicantPreviousCompaniesDto>>(applicantPrevCompaniesJson);
                }
                var currentUserId = HttpContext.User.GetUserId();
                var user = await _uow.UserRepository.GetUserByIdAsync(currentUserId);
                if (user.Credits == 0)
                {

                }
                if (!Enum.IsDefined(typeof(AdDuration), userJobPostDto.AdDuration))
                {
                    return BadRequest("Dužina trajanja oglasa nije ispravna.");
                }
                //userJobPostDto.PricingPlanName = PricingPlanName.Base;
                var validPricingPlans = new List<string> { PricingPlanName.Base, PricingPlanName.Plus, PricingPlanName.Premium };
                if (!validPricingPlans.Contains(userJobPostDto.PricingPlanName))
                {
                    return BadRequest("Paket pretplate oglasa nije ispravna.");
                }

                userJobPostDto.SubmittingUserId = currentUserId;
                if (userJobPostDto.CvFile != null)
                {
                    var fileUrl = await _blobStorageService.UploadFileAsync(userJobPostDto.CvFile);
                    var decodedFileUrl = Uri.UnescapeDataString(fileUrl);
                    userJobPostDto.CvFilePath = decodedFileUrl;
                    userJobPostDto.CvFileName = userJobPostDto.CvFile.FileName;
                }
                else if(userJobPostDto.IsUserProfileCvFileSubmitted == true)
                {
                    userJobPostDto.CvFilePath = user.CvFilePath;
                }

                var newItem = await _jobPostService.CreateUserJobPostAsync(userJobPostDto);
                return Ok(newItem);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpPost("createapplication")]
        public async Task<IActionResult> CreateUserApplication([FromForm] UserApplicationDto userApplication)
        {
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["educations"]))
                {
                    var applicantEducationsJson = Request.Form["educations"];
                    userApplication.Educations = JsonConvert.DeserializeObject<List<UserEducationDto>>(applicantEducationsJson);
                }
                if (!string.IsNullOrEmpty(Request.Form["previousCompanies"]))
                {
                    var applicantPrevCompaniesJson = Request.Form["previousCompanies"];
                    userApplication.PreviousCompanies = JsonConvert.DeserializeObject<List<UserPreviousCompaniesDto>>(applicantPrevCompaniesJson);
                }
                var currentUserId = HttpContext.User.GetUserId();
                var user = await _uow.UserRepository.GetUserByIdAsync(currentUserId);
                userApplication.SubmittingUserId = currentUserId;
                if (userApplication.CvFile != null)
                {
                    var fileUrl = await _blobStorageService.UploadFileAsync(userApplication.CvFile);
                    var decodedFileUrl = Uri.UnescapeDataString(fileUrl);
                    userApplication.CvFilePath = decodedFileUrl;
                    userApplication.CvFileName = userApplication.CvFile.FileName;
                }
                else if (userApplication.IsUserProfileCvFileSubmitted == true)
                {
                    userApplication.CvFilePath = user.CvFilePath;
                    userApplication.CvFileName = user.CvFileName;
                }

                var newItem = await _jobPostService.CreateUserApplicationAsync(userApplication);
                return Ok(newItem);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUserJobPost(int id, [FromForm] UserJobPostDto userJobPostDto)
        {
            userJobPostDto.SubmittingUserId = HttpContext.User.GetUserId();
            if (userJobPostDto.CvFile != null)
            {
                var fileUrl = await _blobStorageService.UploadFileAsync(userJobPostDto.CvFile);
                userJobPostDto.CvFilePath = fileUrl;
            }
            var updatedItem = await _jobPostService.UpdateUserJobPostAsync(userJobPostDto);
            return Ok(updatedItem);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUserJobPost(int id)
        {
            var userId = HttpContext.User.GetUserId();
            var userJob = await _jobPostService.GetUserJobPostByIdAsync(id);
            var valid = await CheckDoesAdBelongsToUser(userJob, userId);
            if (!valid)
                return Unauthorized("Nemate pravo pristupa ovom oglasu");
            var deleted = await _jobPostService.DeleteUserJobPostByIdAsync(userId, id);
            return Ok(deleted);
        }

        [HttpPatch("close/{id}")]
        public async Task<IActionResult> CloseUserJobPost(int id)
        {
            var userId = HttpContext.User.GetUserId();
            var userJob = await _jobPostService.GetUserJobPostByIdAsync(id);
            var valid = await CheckDoesAdBelongsToUser(userJob, userId);
            if (!valid)
                return Unauthorized("Nemate pravo pristupa ovom oglasu");
            var closed = await _jobPostService.CloseUserJobPostByIdAsync(userId, id);
            return Ok(closed);
        }

        [HttpPatch("reactivate/{id}")]
        public async Task<IActionResult> ReactivateUserJobPost(int id)
        {
            var userId = HttpContext.User.GetUserId();
            var userJob = await _jobPostService.GetUserJobPostByIdAsync(id);
            var valid = await CheckDoesAdBelongsToUser(userJob, userId);
            if (!valid)
                return Unauthorized("Nemate pravo pristupa ovom oglasu");
            var reactivated = await _jobPostService.ReactivateUserJobPostByIdAsync(userId, id);
            return Ok(reactivated);
        }

        [HttpGet("types")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllJobTypes()
        {
            var jobTypes = await _jobPostService.GetAllJobTypesAsync();
            return Ok(jobTypes);
        }

        [HttpGet("categories")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllJobCategories()
        {
            var jobCategories = await _jobPostService.GetAllJobCategoriesAsync();
            return Ok(jobCategories);
        }

        [HttpGet("adtypes")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAdTypes()
        {
            var adTypes = await _jobPostService.GetAllAdvertisementTypesAsync();
            return Ok(adTypes);
        }

        [HttpGet("cvfile")]
        public async Task<IActionResult> GetCVFile([FromQuery] int userAdId)
        {
            var userId = HttpContext.User.GetUserId();
            if(userId == null)
            {
                return Forbid("Nemate pravo pristupa.");
            }
            var user = await _uow.UserRepository.GetUserByIdAsync(userId);
            if (user == null || !user.IsCompany)
            {
                return Forbid("Nemate pravo pristupa.");
            }
            var userAd = await _jobPostService.GetUserJobPostByIdAsync(userAdId);
            if (userAd == null)
                return NotFound("Oglas ne postoji!");
            var fileName = Path.GetFileName(userAd.CvFilePath);
            var fileDto = await _blobStorageService.GetFileAsync(fileName);

            if (fileDto.FileContent == null)
            {
                return NotFound("Cv nije pronađen.");
            }

            return File(fileDto.FileContent, fileDto.MimeType, fileName);
        }


        [HttpPatch("baseinfo/{id}")]
        public async Task<IActionResult> UpdateAdUserBaseInfo(int id, [FromBody] UserAdBaseInfoRequest userAdBaseInfoRequest)
        {
            var currentUserId = HttpContext.User.GetUserId();
            var userJob = await _jobPostService.GetUserJobPostByIdAsync(id);
            var valid = await CheckDoesAdBelongsToUser(userJob, currentUserId);
            if (!valid)
                return Unauthorized("Nemate pravo pristupa ovom oglasu");
            userAdBaseInfoRequest.UserAdId = id;
            var updatedItem = await _jobPostService.UpdateAdUserBaseInfo(userAdBaseInfoRequest);
            return Ok(updatedItem);
        }


        [HttpPatch("adinfo/{id}")]
        public async Task<IActionResult> UpdateAdInfo(int id, [FromBody] UserAdInfoUpdateRequest userAdBaseInfoRequest)
        {
            var currentUserId = HttpContext.User.GetUserId();
            var userJob = await _jobPostService.GetUserJobPostByIdAsync(id);
            var valid = await CheckDoesAdBelongsToUser(userJob, currentUserId);
            if (!valid)
                return Unauthorized("Nemate pravo pristupa ovom oglasu");
            userAdBaseInfoRequest.UserAdId = id;
            var updatedItem = await _jobPostService.UpdateAdInfo(userAdBaseInfoRequest);
            return Ok(updatedItem);
        }

        [HttpPost("upsertadeducation/{adId}")]
        public async Task<ActionResult<UserJobPostDto>> UpsertUserEducation(int adId, [FromBody] ApplicantEducationRequest req)
        {
            if (HttpContext.User.GetUserId() == null)
            {
                return Unauthorized("Korisnik ne postoji!");
            }
            if (adId == null || adId == 0)
                return BadRequest("Oglas ne postoji!");
            req.UserId = HttpContext.User.GetUserId();
            req.UserAdId = adId;
            var userJob = await _jobPostService.GetUserJobPostByIdAsync(adId);
            var valid = await CheckDoesAdBelongsToUser(userJob, (int)req.UserId);
            if (!valid)
                return Unauthorized("Nemate pravo pristupa ovom oglasu");
            var result = await _userJobPostRepository.UpsertApplicantEducationAsync(req);
            //var dto = ConvertUserAdToUserAdDto(result);
            return result.ToDto();
        }

        [HttpPost("upsertadcompany/{adId}")]
        public async Task<ActionResult<UserJobPostDto>> UpserUserCompany(int adId, [FromBody] ApplicantCompanyRequst req)
        {
            if (HttpContext.User.GetUserId() == null)
            {
                return Unauthorized("Korisnik ne postoji!");
            }
            if (adId == null || adId == 0)
                return BadRequest("Oglas ne postoji!");
            req.UserId = HttpContext.User.GetUserId();
            req.UserAdId = adId;
            var userJob = await _jobPostService.GetUserJobPostByIdAsync(adId);
            var valid = await CheckDoesAdBelongsToUser(userJob, (int)req.UserId);
            if (!valid)
                return Unauthorized("Nemate pravo pristupa ovom oglasu");
            var result = await _userJobPostRepository.UpsertApplicantCompanyAsync(req);
            return result.ToDto();
        }

        [HttpPost("deleteadeducation")]
        public async Task<ActionResult<UserJobPostDto>> DeleteAdEducation(DeleteApplicantEducationRequest req)
        {
            if (HttpContext.User.GetUserId() == null)
            {
                return Unauthorized("Korisnik ne postoji!");
            }
            var userId = HttpContext.User.GetUserId();
            var userJob = await _jobPostService.GetUserJobPostByIdAsync(req.UserAdId);
            var valid = await CheckDoesAdBelongsToUser(userJob, userId);
            if (!valid)
                return Unauthorized("Nemate pravo pristupa ovom oglasu");

            var adEducations = await _userJobPostRepository.GetAllEducationsByAdIdAsync(req.UserAdId);
            if(adEducations.Where(r => r.Id == req.EducationId).Any())
            {
                var ad = await _userJobPostRepository.DeleteApplicantEducationByIdAsync(req.EducationId);
                //var dto = ConvertUserAdToUserAdDto(ad);
                return ad.ToDto();
            }

            return BadRequest("Desila se greška prilikom brisanja edukacije.");
        }

        [HttpPost("deleteadcompany")]
        public async Task<ActionResult<UserJobPostDto>> DeleteAdCompany(DeleteAplicantCompanyReq req)
        {
            if (HttpContext.User.GetUserId() == null)
            {
                return Unauthorized("Korisnik ne postoji!");
            }
            var userId = HttpContext.User.GetUserId();
            var userJob = await _jobPostService.GetUserJobPostByIdAsync(req.UserAdId);
            var valid = await CheckDoesAdBelongsToUser(userJob, userId);
            if (!valid)
                return Unauthorized("Nemate pravo pristupa ovom oglasu");

            var adCompanies = await _userJobPostRepository.GetAllApplicantCompaniesByAdIdAsync(req.UserAdId);
            if (adCompanies.Where(r => r.Id == req.PrevCompanyId).Any())
            {
                var ad = await _userJobPostRepository.DeleteApplicantCompanyByIdAsync(req.PrevCompanyId);
                //var dto = ConvertUserAdToUserAdDto(ad);
                return ad.ToDto();
            }

            return BadRequest("Desila se greška prilikom brisanja kompanije aplikanta.");
        }

        [HttpPost("uploadfile/{userAdId}")]
        public async Task<ActionResult<UserJobPostDto>> UserAdFileUpload(int userAdId, [FromForm] UserAdFileUploadRequest req)
        {
            if (HttpContext.User.GetUserId() == null)
            {
                return Unauthorized("Korisnik ne postoji!");
            }
            var userId = HttpContext.User.GetUserId();
            var userJob = await _jobPostService.GetUserJobPostByIdAsync(userAdId);
            var valid = await CheckDoesAdBelongsToUser(userJob, userId);
            if (!valid)
                return Unauthorized("Nemate pravo pristupa ovom oglasu");
            if (req.CvFile == null)
            {
                return BadRequest("CV nije uploadovan.");
            }

            if (req.CvFile != null)
            {
                var fileUrl = await _blobStorageService.UploadFileAsync(req.CvFile);
                var decodedFileUrl = Uri.UnescapeDataString(fileUrl);
                var updatedAd = await _userJobPostRepository.UpdateUserAdCvFilePathAsync(userAdId, decodedFileUrl, req.CvFile.FileName);
                var dto = updatedAd.ToDto();
                return dto;
            }

            return BadRequest("Došlo je do greške");
        }

        [HttpGet("deletefile/{userAdId}")]
        public async Task<ActionResult<UserJobPostDto>> UserAdDeleteFile(int userAdId)
        {
            if (HttpContext.User.GetUserId() == null)
            {
                return Unauthorized("Korisnik ne postoji!");
            }
            var userId = HttpContext.User.GetUserId();
            var userJob = await _jobPostService.GetUserJobPostByIdAsync(userAdId);
            var valid = await CheckDoesAdBelongsToUser(userJob, userId);
            if (!valid)
                return Unauthorized("Nemate pravo pristupa ovom oglasu");

            var result = await _userJobPostRepository.DeleteUserAdFileAsync(userAdId);
            //IMPLEMENTE DELETE FILE FROM BLOB
            return result.ToDto();
        }

        [HttpPost("contactuser/{userAdId}")]
        public async Task<IActionResult> SendContactMessage(int userAdId, [FromBody] ContactUserRequestDto request)
        {
            var userId = HttpContext.User.GetUserId();
            if(userId == null)
                return BadRequest(new { message = "Korisnik je obavezan!" });
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(new { message = "Email i poruka su obavezni!" });
            }

            var userApplication = await _jobPostService.GetUserJobPostByIdAsync(userAdId);
            var user = await _uow.UserRepository.GetUserByIdAsync(userId);
            if (user == null || userApplication == null)
                return BadRequest(new { message = "Korisnik nije pronadjen!" });

            var now = DateTime.UtcNow;
            var contactUserRequest = new ContactUserRequest()
            {
                Email = request.Email,
                Message = request.Message,
                Phone = request.Phone,
                Subject = request.Subject,
                UserJobPostId = userAdId,
                FromUserId = userId,
                CreatedAt = now,
                CompanyName = user.Company?.CompanyName ?? "Ponuda od drugog korisnika",
                ToUserId = userApplication.SubmittingUserId
            };

            await _dbContext.ContactUserRequests.AddAsync(contactUserRequest);
            await _dbContext.SaveChangesAsync();

            var notification = new Notification()
            {
                UserId = userApplication.SubmittingUserId.ToString(),
                CreatedAt = now,
                IsRead = false,
                Link = UIBaseUrl + $"user-settings/job-offers/{contactUserRequest.Id}",
                Message = "Nova poslovna prilika! Poslodavac Vas želi kontaktirati"
            };
            _dbContext.Notifications.Add(notification);

            await _dbContext.SaveChangesAsync();

            await SendNewContactUserEmailAsync(userApplication, contactUserRequest.Id, request.Message, request.Subject, request.Phone, request.Email);

            return Ok(new { message = "Poruka je uspešno poslana!" });
        }

        [HttpGet("userjoboffers")]
        public async Task<IActionResult> GetAllUserJobOffers()
        {
            var currentUserId = HttpContext.User.GetUserId();
            if (currentUserId == null)
                return Unauthorized("Nemate pravo pristupa");
            var userJobOffers = await _dbContext.ContactUserRequests.Where(r => r.ToUserId == currentUserId).Include(r => r.UserJobPost).OrderByDescending(r => r.CreatedAt).ToListAsync();
            var userJobOffersTableData = new List<UserJobOffersTableData>();
            foreach (var jobOffer in userJobOffers)
            {
                var tableData = new UserJobOffersTableData()
                {
                    JobPosition = jobOffer.UserJobPost.Position,
                    CreatedAt = jobOffer.CreatedAt,
                    CompanyName = jobOffer.CompanyName,
                    UserJobPostId = jobOffer.UserJobPostId,
                    Phone = jobOffer.Phone,
                    Email = jobOffer.Email,
                    Message = jobOffer.Message,
                    Subject = jobOffer.Subject,
                    Id = jobOffer.Id
                };
                userJobOffersTableData.Add(tableData);
            }
            return Ok(userJobOffersTableData);
        }

        private async Task SendNewContactUserEmailAsync(UserJobPostDto userAd, int contactUserId, string message, string requestSubject, string phone, string email)
        {
            if (userAd != null)
            {
                var applicationUrl = UIBaseUrl + $"user-settings/job-offers/{contactUserId}";

                string messageBody = $@"
            <h4 style='color: black;'>Naslov: {requestSubject}</h4>
            <p style='color: #66023C;'>Poruka: {message}</p>
            <p style='color: #66023C;'>Email poslodavca: {email}</p>
            <p style='color: #66023C;'>Broj telefona poslodavca: {phone}</p>
            <p style='text-align: center;'>
                <a href='{applicationUrl}' style='display: inline-block; padding: 10px 20px; background-color: #66023C; color: #ffffff; text-decoration: none; border-radius: 5px;'>Pogledajte oglas</a>
            </p>";

                var subject = "Poslovni oglasi - Nova poslovna prilika! Poslodavac Vas želi kontaktirati";
                var emailTemplate = EmailTemplateHelper.GenerateEmailTemplate(subject, messageBody, _configuration);

                try
                {
                    await emailService.SendEmailWithTemplateAsync(userAd.ApplicantEmail, subject, emailTemplate);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        private UserJobPostDto ConvertUserAdToUserAdDto(UserJobPost userJobPost)
        {
            var dto = new UserJobPostDto
            {
                ApplicantFirstName = userJobPost.ApplicantFirstName,
                ApplicantLastName = userJobPost.ApplicantLastName,
                ApplicantPhoneNumber = userJobPost.ApplicantPhoneNumber,
                ApplicantDateOfBirth = (DateTime)userJobPost.ApplicantDateOfBirth,
                AdAdditionalDescription = userJobPost.AdAdditionalDescription,
                //AdDuration = userJobPost.Dur
                AdEndDate = userJobPost.AdEndDate,
                AdStartDate = userJobPost.AdStartDate,
                AdTitle = userJobPost.AdTitle,
                AdvertisementTypeId = userJobPost.AdvertisementTypeId,
                ApplicantGender = userJobPost.ApplicantGender == Gender.Male ? "Male" : userJobPost.ApplicantGender == Gender.Female ? "Female" : "Other",
                Id = userJobPost.Id,
                Price = userJobPost.Price,
                ApplicantEducations = userJobPost.ApplicantEducations?.Select(r => new ApplicantEducationDto()
                {
                    Degree = r.Degree,
                    EducationEndYear = r.EducationEndYear,
                    EducationStartYear = r.EducationStartYear,
                    FieldOfStudy = r.FieldOfStudy,
                    University = r.University,
                    InstitutionName = r.University,
                    UserEducationId = userJobPost.Id
                }).ToList() ?? new List<ApplicantEducationDto>()
            };
            return dto;
        }
    }
}
