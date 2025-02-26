using API.Data;
using API.Data.ICompanyJobPostRepository;
using API.DTOs;
using API.Entities.Applications;
using API.Extensions;
using API.Helpers;
using API.Mappers;
using API.PaginationEntities;
using API.Services;
using API.Services.CompanyJobPostServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    public class CompanyJobController : BaseController
    {
        private readonly ICompanyJobPostService _jobPostService;
        private readonly IUnitOfWork _uow;
        private readonly ICompanyJobPostRepository _jobPostRepository;
        private readonly IUserApplicationsRepository _userApplicationsRepository;
        private readonly DataContext _dbContext;
        private readonly IBlobStorageService _blobStorageService;
        private readonly ISendNotificationsQueueClient _sendNotificationsQueueClient;

        public CompanyJobController(ICompanyJobPostService jobPostService, IUnitOfWork uow, ICompanyJobPostRepository companyJobPostRepository, IUserApplicationsRepository userApplicationsRepository, DataContext dbContext, IBlobStorageService blobStorageService, ISendNotificationsQueueClient sendNotificationsQueueClient)
        {
            _jobPostService = jobPostService;
            _uow = uow;
            _jobPostRepository = companyJobPostRepository;
            _userApplicationsRepository = userApplicationsRepository;
            _dbContext = dbContext;
            _blobStorageService = blobStorageService;
            _sendNotificationsQueueClient = sendNotificationsQueueClient;
        }


        [HttpPost("registeredcompanies")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRegisteredCompanies([FromBody] AdsPaginationParameters adsParameters)
        {
            var registeredCompanies = await _jobPostService.GetRegisteredCompaniesAsync(adsParameters);
            var pagedResponse = registeredCompanies.ToPagedResponse();
            return Ok(pagedResponse);
        }

        [HttpPost("adsprivate")]
        public async Task<IActionResult> GetAds([FromBody] AdsPaginationParameters adsParameters)
        {
            adsParameters.adStatus = JobPostStatus.Active;
            var jobPosts = await _jobPostService.GetJobPostsAsync(adsParameters);
            var pagedResponse = jobPosts.ToPagedResponse();
            var currentUserId = HttpContext.User.GetUserId();
            if(currentUserId != null && currentUserId != 0)
            {
                var user = await _uow.UserRepository.GetUserByIdAsync(currentUserId);
                
                foreach (var item in pagedResponse.Items)
                {
                    if ((item.UsersThatAppliedOnJobPost.Any() && item.UsersThatAppliedOnJobPost.Contains(currentUserId)) || user.UserRoles.Select(r => r.Role.Name).Contains("Company"))
                        item.CanCurrentUserApplyOnAd = false;
                }
            }
            return Ok(pagedResponse);
        }

        [HttpPost("adspublic")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAdsPublic([FromBody] AdsPaginationParameters adsParameters)
        {
            adsParameters.adStatus = JobPostStatus.Active;
            var jobPosts = await _jobPostService.GetJobPostsAsync(adsParameters);
            var pagedResponse = jobPosts.ToPagedResponse();
            return Ok(pagedResponse);
        }

        [HttpGet("employmenttypes")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEmploymentTypes()
        {
            var employmentTypes = await _jobPostRepository.GetEmploymentTypesAsync();
            return Ok(employmentTypes);
        }

        [HttpGet("educationlevels")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEducationLevels()
        {
            var educationLevels = await _jobPostRepository.GetEducationLevels();
            return Ok(educationLevels);
        }

        [HttpGet("employmentstatuses")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEmploymentStatuses()
        {
            var employmentStatuses = await _jobPostRepository.GetEmploymentStatusesAsync();
            return Ok(employmentStatuses);
        }

        [HttpPost("company-ads")]
        public async Task<IActionResult> GetCompanyAds([FromBody] AdsPaginationParameters adsParameters)
        {
            var currentUserId = HttpContext.User.GetUserId();
            adsParameters.UserId = currentUserId;
            var user = await _uow.UserRepository.GetUserByIdAsync(currentUserId);
            if (user == null || user.CompanyId == null)
                return NotFound("Ne pripadata niti jednoj kompaniji.");
            adsParameters.CompanyId = user.CompanyId;
            var myAds = await _jobPostService.GetCompanyJobPostsAsync(adsParameters);
            var pagedResponse = myAds.ToPagedResponse();
            return Ok(pagedResponse);
        }

        [HttpGet("company-job/{id}")]
        public async Task<IActionResult> GetCompanyJobById(int id)
        {
            var companyJob = await _jobPostService.GetCompanyJobPostByIdAsync(id);
            var currentUserId = HttpContext.User.GetUserId();
            var user = await _uow.UserRepository.GetUserByIdAsync(currentUserId);
            if (currentUserId != null && currentUserId != 0)
            {
                companyJob.CanCurrentUserApplyOnAd = companyJob.UsersThatAppliedOnJobPost.Contains(currentUserId) || user.UserRoles.Select(r => r.Role.Name).Contains("Company") ? false : true;
            }
            if (companyJob.IsDeleted || companyJob.JobPostStatusId != (int)JobPostStatus.Active || companyJob.AdEndDate < DateTime.Now)
                return NotFound("Oglas je obrisan, zatvoren, ili je istekao.");
            return Ok(companyJob);
        }

        [HttpGet("company-job-public/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCompanyJobByIdPublic(int id)
        {
            var companyJob = await _jobPostService.GetCompanyJobPostByIdAsync(id);
            if (companyJob.IsDeleted || companyJob.JobPostStatusId != (int)JobPostStatus.Active || companyJob.AdEndDate < DateTime.Now)
                return NotFound("Oglas je obrisan, zatvoren, ili je istekao.");
            return Ok(companyJob);
        }

        [HttpGet("company-my-ad/{id}")]
        public async Task<IActionResult> GetCompanyMyJobById(int id)
        {
            var currentUserId = HttpContext.User.GetUserId();
            var companyJob = await _jobPostService.GetCompanyJobPostByIdAsync(id);
            if (companyJob != null && companyJob.SubmittingUserId != currentUserId)
                return Unauthorized("Nemate pravo pristupa ovom oglasu");
            return Ok(companyJob);
        }

        [HttpGet("company-active-jobs")]
        public async Task<IActionResult> GetCompanyActiveJobs()
        {
            var userId = HttpContext.User.GetUserId();
            var user = await _uow.UserRepository.GetUserByIdAsync(userId);
            if (user == null || user.CompanyId == null)
                return Unauthorized("You do not belong to the current company.");
            
            
            var companyJobs = await _jobPostRepository.GetCompanyActiveAdsAsync((int)user.CompanyId);
            var dtos = companyJobs.ToDto();
            foreach(var item in dtos)
            {
                item.PricingPlanName = companyJobs.First(r => r.Id == item.Id).PricingPlan.Name;
            }
            return Ok(dtos);
        }

        [HttpPost("runaiforallcandidates/{jobId}")]
        public async Task<IActionResult> RunAIForAllCandidatesOfCompanyJobPost(int jobId)
        {
            try
            {
                var userId = HttpContext.User.GetUserId();
                var user = await _uow.UserRepository.GetUserByIdAsync(userId);
                if (user == null || user.CompanyId == null)
                    return Unauthorized("You do not belong to the current company.");

                var companyId = user.CompanyId;
                var companyJobPost = await _jobPostRepository.GetCompanyJobPostByIdAsync(jobId);
                if (companyJobPost == null)
                    return NotFound();
                if (companyJobPost.User.CompanyId != companyId)
                    return Unauthorized("Not belong to current company");

                var userApplicationsForJobPost = await _userApplicationsRepository.GetApplicationsForSpecificCompanyJobPost(jobId);
                var applicationsThatAreNotProcessedByAi = userApplicationsForJobPost.Where(r => r.AIMatchingResult == null).Select(r => r.Id).ToList();

                companyJobPost.IsAiAnalysisIncluded = true;
                await _dbContext.SaveChangesAsync();

                if (applicationsThatAreNotProcessedByAi != null && applicationsThatAreNotProcessedByAi.Any())
                {
                    var applicantPredictionMessage = new NewApplicantPredictionQueueMessage()
                    {
                        CompanyJobPostId = companyJobPost.Id,
                        UserApplicationIds = applicationsThatAreNotProcessedByAi,
                    };

                    await _sendNotificationsQueueClient.SendNewApplicantPredictionMessageAsync(applicantPredictionMessage);

                    companyJobPost.AiAnalysisProgress = 0.00;
                    companyJobPost.AiAnalysisStartedOn = DateTime.UtcNow;
                    await _dbContext.SaveChangesAsync();

                    return Ok(true);
                }
                return NotFound(false);
            }
            catch(Exception ex)
            {
                return StatusCode(500, false);
            }
        }

        [HttpGet("aianalysisstatuspolling/{jobId}")]
        public async Task<IActionResult> AIAnalysisForAllCandidatesStatusPolling(int jobId)
        {
            try
            {
                var userId = HttpContext.User.GetUserId();
                if (userId == null)
                    return Unauthorized("You do not belong to the current company.");
                var companyJobPost = await _jobPostRepository.GetCompanyJobPostByIdAsync(jobId);
                if (companyJobPost == null)
                    return NotFound();

                var res = new AIAnalysisPollingResponse()
                {
                    Error = companyJobPost.AiAnalysisError,
                    HasError = companyJobPost.AiAnalysisHasError ?? false,
                    Progress = companyJobPost.AiAnalysisProgress
                };
                return Ok(res); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching AI analysis status.");
            }
        }


        [HttpGet("company-job-candidates/{jobId}")]
        public async Task<IActionResult> GetCompanyJobCandidates(int jobId)
        {
            var userId = HttpContext.User.GetUserId();
            var user = await _uow.UserRepository.GetUserByIdAsync(userId);
            if (user == null || user.CompanyId == null)
                return Unauthorized("You do not belong to the current company.");

            var companyId = user.CompanyId;
            var companyJobPost = await _jobPostRepository.GetCompanyJobPostByIdAsync(jobId);
            if (companyJobPost == null)
                return NotFound();
            if (companyJobPost.User.CompanyId != companyId)
                return Unauthorized("Not belong to current company");
            //if (companyJobPost.JobPostStatusId != 1 || companyJobPost.AdEndDate < DateTime.Now)
            //    return BadRequest("Oglas je istekao");

            var userApplicationsForJobPost = await _userApplicationsRepository.GetApplicationsForSpecificCompanyJobPost(jobId);
            var conversations = await _dbContext.Conversations.Where(r => r.CompanyJobPostId == jobId).ToListAsync();
            var candidatesTableData = new List<JobCandidatesTableDataDto>();
            foreach (var application in userApplicationsForJobPost)
            {
                var tableData = new JobCandidatesTableDataDto()
                {
                    CandidateEmail = application.Email,
                    ApplicationStatusId = application.ApplicationStatusId,
                    UserApplicationId = application.Id,
                    CandidateCity = application.City?.Name,
                    CandidateCoverLetter = application.CoverLetter,
                    CandidateDateOfBirth = application.DateOfBirth,
                    CandidateFullName = application.LastName + " " + application.FirstName,
                    CandidateGender = application.Gender,
                    CandidatePhoneNumber = application.PhoneNumber,
                    CvFilePath = application.CvFilePath,
                    ApplicationDate = application.CreatedAt,
                    MeetingDateTime = application.MeetingDateTime,
                    IsOnlineMeeting = application.IsOnlineMeeting,
                    AIMatchingResult = application.AIMatchingResult,
                    AIMatchingSkills = application.AIMatchingSkills,
                    AIMatchingDescription = application.AIMatchingDescription,
                    AIMatchingEducationLevel = application.AIMatchingEducationLevel,
                    AIMatchingExperience = application.AIMatchingExperience,
                    ConversationId = conversations.FirstOrDefault(r => r.ToUserId == application.SubmittingUserId)?.Id
                };
                candidatesTableData.Add(tableData);
            }
            return Ok(candidatesTableData);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCompanyJobPost([FromForm] CompanyJobPostDto companyJobPostDto)
        {
            var userId = HttpContext.User.GetUserId();
            if (userId == null)
                return Forbid("Niste autorizovani za ovu funkcionalnost.");
            companyJobPostDto.SubmittingUserId = userId;
            if(companyJobPostDto.Logo != null)
            {
                var fileUrl = await _blobStorageService.UploadFileAsync(companyJobPostDto.Logo);
                var decodedFileUrl = Uri.UnescapeDataString(fileUrl);
                companyJobPostDto.PhotoUrl = decodedFileUrl;
            }
            var newItem = await _jobPostService.CreateCompanyJobPostAsync(companyJobPostDto);
            return Ok(newItem);
        }

        [HttpPost("uploadLogo/{id}")]
        public async Task<IActionResult> UploadAdLogo(int id, IFormFile photo)
        {
            var userId = HttpContext.User.GetUserId();
            var user = await _uow.UserRepository.GetUserByIdAsync(userId);
            if (user == null || user.CompanyId == null)
                return Unauthorized("You do not belong to the current company.");
            var companyJob = await _jobPostService.GetCompanyJobPostByIdAsync(id);
            var valid = await CheckDoesAdBelongsToUser(companyJob, userId);
            if (!valid)
                return Unauthorized("Nemate pravo pristupa ovom oglasu");
            try
            {
                var existingLogoUrl = companyJob.PhotoUrl;
                //if(existingLogoUrl != null)
                //    await _blobStorageService.RemoveFileAsync(existingLogoUrl);
                var fileUrl = await _blobStorageService.UploadFileAsync(photo);
                var decodedFileUrl = Uri.UnescapeDataString(fileUrl);
                companyJob.PhotoUrl = decodedFileUrl;
                var updateLogo = await _jobPostService.UpdateCompanyJobPostLogoAsync(id, decodedFileUrl);
                return Ok(companyJob);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("removeLogo/{id}")]
        public async Task<IActionResult> RemoveAdLogo(int id)
        {
            var userId = HttpContext.User.GetUserId();
            var user = await _uow.UserRepository.GetUserByIdAsync(userId);
            if (user == null || user.CompanyId == null)
                return Unauthorized("You do not belong to the current company.");
            var companyJob = await _jobPostService.GetCompanyJobPostByIdAsync(id);
            var valid = await CheckDoesAdBelongsToUser(companyJob, userId);
            if (!valid)
                return Unauthorized("Nemate pravo pristupa ovom oglasu");
            try
            {
                companyJob.PhotoUrl = null;
                var updateLogo = await _jobPostService.UpdateCompanyJobPostLogoAsync(id, null);
                return Ok(companyJob);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateCompanyJobPost(int id, [FromBody] CompanyJobPostDto companyJobPostDto)
        {
            var userId = HttpContext.User.GetUserId();
            var user = await _uow.UserRepository.GetUserByIdAsync(userId);
            if (user == null || user.CompanyId == null)
                return Unauthorized("You do not belong to the current company.");
            var updatedItem = await _jobPostService.UpdateCompanyJobPostAsync(companyJobPostDto);
            return Ok(updatedItem);
        }

        [HttpPatch("adinfo/{id}")]
        public async Task<IActionResult> UpdateAdInfo(int id, [FromBody] CompanyUpdateAdInfoRequest companyJobPostDto)
        {
            var currentUserId = HttpContext.User.GetUserId();
            var user = await _uow.UserRepository.GetUserByIdAsync(currentUserId);
            if (user == null || user.CompanyId == null)
                return Unauthorized("You do not belong to the current company.");
            var companyJob = await _jobPostService.GetCompanyJobPostByIdAsync(id);
            var valid = await CheckDoesAdBelongsToUser(companyJob, currentUserId);
            if (!valid)
                return Unauthorized("Nemate pravo pristupa ovom oglasu");
            companyJob.CityId = companyJobPostDto.CityId;
            companyJob.JobCategoryId = companyJobPostDto.JobCategoryId;
            companyJob.JobDescription = companyJobPostDto.JobDescription;
            companyJob.JobTypeId = companyJobPostDto.JobTypeId;
            companyJob.EmailForReceivingApplications = companyJobPostDto.EmailForReceivingApplications;
            companyJob.Position = companyJobPostDto.Position;
            companyJob.CompanyName = companyJobPostDto.CompanyName;
            var updatedItem = await _jobPostService.UpdateCompanyJobPostAsync(companyJob);
            return Ok(updatedItem);
        }

        [HttpPatch("compensation-and-environment/{id}")]
        public async Task<IActionResult> UpdateCompensationAndWorkEnvInfo(int id, [FromBody] CompanyUpdateCompensationAndWorkEnvRequest companyJobPostDto)
        {
            var currentUserId = HttpContext.User.GetUserId();
            var user = await _uow.UserRepository.GetUserByIdAsync(currentUserId);
            if (user == null || user.CompanyId == null)
                return Unauthorized("You do not belong to the current company.");
            var companyJob = await _jobPostService.GetCompanyJobPostByIdAsync(id);
            var valid = await CheckDoesAdBelongsToUser(companyJob, currentUserId);
            if (!valid)
                return Unauthorized("Nemate pravo pristupa ovom oglasu");
            companyJob.MinSalary = companyJobPostDto.MinSalary;
            companyJob.MaxSalary = companyJobPostDto.MaxSalary;
            companyJob.Benefits = companyJobPostDto.Benefits;
            companyJob.WorkEnvironmentDescription = companyJobPostDto.WorkEnvironmentDescription;
            var updatedItem = await _jobPostService.UpdateCompensationAndWorkEnvAsync(companyJob);
            return Ok(updatedItem);
        }

        [HttpPatch("qualifications-and-experience/{id}")]
        public async Task<IActionResult> UpdateQualificationsAndExperience(int id, [FromBody] CompanyUpdateQualificationsAndExperienceRequest companyJobPostDto)
        {
            var currentUserId = HttpContext.User.GetUserId();
            var user = await _uow.UserRepository.GetUserByIdAsync(currentUserId);
            if (user == null || user.CompanyId == null)
                return Unauthorized("You do not belong to the current company.");
            var companyJob = await _jobPostService.GetCompanyJobPostByIdAsync(id);
            var valid = await CheckDoesAdBelongsToUser(companyJob, currentUserId);
            if (!valid)
                return Unauthorized("Nemate pravo pristupa ovom oglasu");
            companyJob.RequiredSkills = companyJobPostDto.RequiredSkills;
            companyJob.EducationLevelId = companyJobPostDto.EducationLevelId;
            companyJob.Certifications = companyJobPostDto.Certifications;
            companyJob.RequiredExperience = companyJobPostDto.RequiredExperience;
            var updatedItem = await _jobPostService.UpdateQualificationsAndExpereinceAsync(companyJob);
            return Ok(updatedItem);
        }

        [HttpPatch("howtoapply/{id}")]
        public async Task<IActionResult> UpdateHowToApply(int id, [FromBody] CompanyUpdateHowToApplyRequest companyJobPostDto)
        {
            var currentUserId = HttpContext.User.GetUserId();
            var user = await _uow.UserRepository.GetUserByIdAsync(currentUserId);
            if (user == null || user.CompanyId == null)
                return Unauthorized("You do not belong to the current company.");
            var companyJob = await _jobPostService.GetCompanyJobPostByIdAsync(id);
            var valid = await CheckDoesAdBelongsToUser(companyJob, currentUserId);
            if (!valid)
                return Unauthorized("Nemate pravo pristupa ovom oglasu");
            companyJob.HowToApply = companyJobPostDto.HowToApply;
            companyJob.DocumentsRequired = companyJobPostDto.DocumentsRequired;
            var updatedItem = await _jobPostService.UpdateHowToApplyAsync(companyJob);
            return Ok(updatedItem);
        }

        private async Task<bool> CheckDoesAdBelongsToUser(CompanyJobPostDto companyJob, int userId)
        {
            if (companyJob != null && companyJob.SubmittingUserId != userId)
                return false;
            return true;
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCompanyJobPost(int id)
        {
            var userId = HttpContext.User.GetUserId();
            var user = await _uow.UserRepository.GetUserByIdAsync(userId);
            if (user == null || user.CompanyId == null)
                return Unauthorized("You do not belong to the current company.");
            var companyJob = await _jobPostService.GetCompanyJobPostByIdAsync(id);
            var valid = await CheckDoesAdBelongsToUser(companyJob, userId);
            if (!valid)
                return Unauthorized("Nemate pravo pristupa ovom oglasu");
            var deleted = await _jobPostService.DeleteCompanyJobPostByIdAsync((int)user.CompanyId, id);
            return Ok(deleted);
        }

        [HttpPatch("close/{id}")]
        public async Task<IActionResult> CloseCompanyJobPost(int id)
        {
            var userId = HttpContext.User.GetUserId();
            var user = await _uow.UserRepository.GetUserByIdAsync(userId);
            if (user == null || user.CompanyId == null)
                return Unauthorized("You do not belong to the current company.");
            var companyJob = await _jobPostService.GetCompanyJobPostByIdAsync(id);
            var valid = await CheckDoesAdBelongsToUser(companyJob, userId);
            if (!valid)
                return Unauthorized("Nemate pravo pristupa ovom oglasu");
            var closed = await _jobPostService.CloseCompanyJobPostByIdAsync((int)user.CompanyId, id);
            return Ok(closed);
        }

        [HttpPatch("reactivate/{id}")]
        public async Task<IActionResult> ReactivateCompanyJobPost(int id)
        {
            var userId = HttpContext.User.GetUserId();
            var user = await _uow.UserRepository.GetUserByIdAsync(userId);
            if (user == null || user.CompanyId == null)
                return Unauthorized("You do not belong to the current company.");
            var companyJob = await _jobPostService.GetCompanyJobPostByIdAsync(id);
            var valid = await CheckDoesAdBelongsToUser(companyJob, userId);
            if (!valid)
                return Unauthorized("Nemate pravo pristupa ovom oglasu");
            var reactivated = await _jobPostService.ReactivateCompanyJobPostByIdAsync((int)user.CompanyId, id);
            return Ok(reactivated);
        }
    }
}
