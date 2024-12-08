using API.Data;
using API.Data.IUserOfferRepository;
using API.DTOs;
using API.Entities.JobPost;
using API.Extensions;
using API.Helpers;
using API.Mappers;
using API.PaginationEntities;
using API.Services;
using API.Services.UserOfferServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace API.Controllers
{
    //[Authorize]
    public class JobController : BaseController
    {
        private readonly IUserJobPostService _jobPostService;
        private readonly IUnitOfWork _uow;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IUserJobPostRepository _userJobPostRepository;

        public JobController(IUserJobPostService jobPostService, IUnitOfWork uow, IBlobStorageService blobStorageService, IUserJobPostRepository userJobPostRepository)
        {
            _jobPostService = jobPostService;
            _uow = uow;
            _blobStorageService = blobStorageService;
            _userJobPostRepository = userJobPostRepository;
        }

        [HttpPost("ads")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAds([FromBody] AdsPaginationParameters adsParameters)
        {
            var jobPosts = await _jobPostService.GetJobPostsAsync(adsParameters);
            var pagedResponse = jobPosts.ToPagedResponse();
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
        public async Task<IActionResult> GetUserJobById(int id)
        {
            var userJob = await _jobPostService.GetUserJobPostByIdAsync(id);
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
        public async Task<IActionResult> GetAllJobTypes()
        {
            var jobTypes = await _jobPostService.GetAllJobTypesAsync();
            return Ok(jobTypes);
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetAllJobCategories()
        {
            var jobCategories = await _jobPostService.GetAllJobCategoriesAsync();
            return Ok(jobCategories);
        }

        [HttpGet("adtypes")]
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
            if (user == null) // || user.IsCompany with active subscription
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
