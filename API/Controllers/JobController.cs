using API.Data;
using API.Data.IUserOfferRepository;
using API.DTOs;
using API.Entities;
using API.Entities.Chat;
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
            var contactedAdsByCurrentUser = await _dbContext.Conversations.Where(r => r.FromUserId == userId).ToListAsync();
            var contactedAdsByCurrentUserIds = contactedAdsByCurrentUser.Select(r => r.Id);
            var currentUser = await _uow.UserRepository.GetUserByIdAsync(userId);
            foreach(var item in pagedResponse.Items)
            {
                if (contactedAdsByCurrentUserIds.Contains(item.Id) || item.SubmittingUserId == userId || !currentUser.IsCompany)
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
            var contactedAdsByCurrentUserIds = await _dbContext.Conversations.Where(r => r.FromUserId == userId).Select(r => r.FromUserId).ToListAsync();
            var currentUser = await _uow.UserRepository.GetUserByIdAsync(userId);
            if (contactedAdsByCurrentUserIds.Contains(userJob.Id) || userJob.SubmittingUserId == userId || !currentUser.IsCompany)
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
            var userAd = await _jobPostService.GetUserJobPostByIdAsync(userAdId);
            if (user == null || (!user.IsCompany && user.Id != userAd.SubmittingUserId))
            {
                return Forbid("Nemate pravo pristupa.");
            }
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
