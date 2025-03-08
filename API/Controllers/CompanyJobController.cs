using API.Data;
using API.Data.ICompanyJobPostRepository;
using API.DTOs;
using API.Entities.Applications;
using API.Entities.CompanyJobPost;
using API.Extensions;
using API.Helpers;
using API.Mappers;
using API.PaginationEntities;
using API.Services;
using API.Services.CompanyJobPostServices;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<CompanyJobController> _logger;

        public CompanyJobController(ICompanyJobPostService jobPostService, IUnitOfWork uow, ICompanyJobPostRepository companyJobPostRepository, IUserApplicationsRepository userApplicationsRepository, DataContext dbContext, IBlobStorageService blobStorageService, ISendNotificationsQueueClient sendNotificationsQueueClient, ILogger<CompanyJobController> logger)
        {
            _jobPostService = jobPostService;
            _uow = uow;
            _jobPostRepository = companyJobPostRepository;
            _userApplicationsRepository = userApplicationsRepository;
            _dbContext = dbContext;
            _blobStorageService = blobStorageService;
            _sendNotificationsQueueClient = sendNotificationsQueueClient;
            _logger = logger;
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

                //if (companyJobPost.AiAnalysisHasError == true || (companyJobPost.AiAnalysisHasError == false || companyJobPost.AiAnalysisHasError) && companyJobPost.AiAnalysisProgress > 0 && companyJobPost.AiAnalysisProgress < 100)
                //    return BadRequest("Analysis is running or has error");

                var userApplicationsForJobPost = await _userApplicationsRepository.GetApplicationsForSpecificCompanyJobPost(jobId);
                var applicationsThatAreNotProcessedByAi = userApplicationsForJobPost.Where(r => r.AIMatchingResult == null).Select(r => r.Id).ToList();
                

                if (applicationsThatAreNotProcessedByAi != null && applicationsThatAreNotProcessedByAi.Any())
                {
                    var totalAIAnalysisPrice = Math.Round(applicationsThatAreNotProcessedByAi.Count() * 0.1, 1);
                    var userCredits = user.Credits;
                    if (userCredits < totalAIAnalysisPrice)
                        return BadRequest("Nemate dovoljno kredita za izvršavanje AI analize. Molimo Vas da dopunite kredite, te nakon toga pokrenete AI analizu.");

                    //companyJobPost.IsAiAnalysisIncluded = true;
                    companyJobPost.AiAnalysisReservedCredits = totalAIAnalysisPrice;
                    await _dbContext.SaveChangesAsync();

                    var applicantPredictionMessage = new NewApplicantPredictionQueueMessage()
                    {
                        CompanyJobPostId = companyJobPost.Id,
                        UserApplicationIds = applicationsThatAreNotProcessedByAi,
                        UserId = user.Id,
                        ReservedCredits = totalAIAnalysisPrice
                    };

                    await _sendNotificationsQueueClient.SendNewApplicantPredictionMessageAsync(applicantPredictionMessage);

                    user.Credits -= totalAIAnalysisPrice;
                    //companyJobPost.AiAnalysisProgress = 0.1;
                    companyJobPost.AiAnalysisStartedOn = DateTime.UtcNow;
                    companyJobPost.AiAnalysisStatus = Entities.CompanyJobPost.AiAnalysisStatus.Running;
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
                var user = await _uow.UserRepository.GetUserByIdAsync(userId);
                if (user == null || user.CompanyId == null)
                    return Unauthorized("You do not belong to the current company.");

                var companyJobPost = await _jobPostRepository.GetCompanyJobPostByIdAsync(jobId);
                if (companyJobPost == null)
                    return NotFound();

                var companyId = user.CompanyId;
                if (companyJobPost.User == null || companyJobPost.User.CompanyId != companyId)
                    return Unauthorized("Not belong to current company");

                var aiAnalysisStartedOn = companyJobPost.AiAnalysisStartedOn;
                var now = DateTime.UtcNow;

                if (aiAnalysisStartedOn.HasValue && aiAnalysisStartedOn.Value.AddMinutes(15) < now
                    && companyJobPost.AiAnalysisStatus == Entities.CompanyJobPost.AiAnalysisStatus.Running)
                {
                    //Use transaction with isolation here to ensure that there is no race condition(2 users from different machines pings this endpoint)
                    using (var transaction = await _dbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable))
                    {
                        try
                        {
                            companyJobPost.AiAnalysisError = "Desila se greška prilikom izvršavanja AI analize. Unutar 15 minuta analiza se nije uspješno izvršila.";
                            companyJobPost.AiAnalysisHasError = true;
                            companyJobPost.AiAnalysisEndedOn = DateTime.UtcNow;
                            companyJobPost.AiAnalysisStatus = Entities.CompanyJobPost.AiAnalysisStatus.Completed;

                            //Ensure credits are only refunded once
                            if (companyJobPost.AiAnalysisReservedCredits.HasValue)
                            {
                                user.Credits += companyJobPost.AiAnalysisReservedCredits.Value;
                                companyJobPost.AiAnalysisReservedCredits = null;
                            }

                            await _dbContext.SaveChangesAsync();
                            await transaction.CommitAsync(); //Commit transaction only after all operations succeed
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync(); //Rollback transaction if anything fails
                            return StatusCode(500, "An error occurred while processing AI analysis status.");
                        }
                    }
                }

                var res = new AIAnalysisPollingResponse()
                {
                    Error = companyJobPost.AiAnalysisError,
                    HasError = companyJobPost.AiAnalysisHasError ?? false,
                    Status = companyJobPost.AiAnalysisStatus.HasValue ? companyJobPost.AiAnalysisStatus.Value : AiAnalysisStatus.Completed,
                    UserCredits = user.Credits
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

            var allPreviousCompanyJobPostsIds = await _dbContext.CompanyJobPosts.Where(r => r.SubmittingUserId == user.Id && r.Id != companyJobPost.Id).Select(r => r.Id).ToListAsync();
            List<int> getApplicantsForAllCompanyJobPosts = new List<int>();

            if (allPreviousCompanyJobPostsIds.Any())
            {
                // If there are other job posts, fetch the applicants who applied to those job posts
                getApplicantsForAllCompanyJobPosts = await _dbContext.UserApplications
                    .Where(r => allPreviousCompanyJobPostsIds.Contains(r.CompanyJobPostId))
                    .Select(r => r.SubmittingUserId)
                    .ToListAsync();
            }

            var candidatesTableData = new List<JobCandidatesTableDataDto>();
            foreach (var application in userApplicationsForJobPost)
            {
                var tableData = new JobCandidatesTableDataDto()
                {
                    UserId = application.SubmittingUserId,
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
                    ConversationId = conversations.FirstOrDefault(r => r.ToUserId == application.SubmittingUserId)?.Id,
                    DidUserApplyOnPreviousCompanyJobPosts = getApplicantsForAllCompanyJobPosts.Contains(application.SubmittingUserId)
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
            if (companyJobPostDto.Logo != null && !FileHelper.IsValidImage(companyJobPostDto.Logo))
            {
                return BadRequest("Nevažeći format slike. Dozvoljeni formati: JPG, PNG, GIF, BMP, WEBP.");
            }
            var user = await _uow.UserRepository.GetUserByIdAsync(userId);
            var pricingPlan = await _dbContext.PricingPlanCompanies.FirstOrDefaultAsync(r => r.Name.Equals(companyJobPostDto.PricingPlanName) && r.AdActiveDays == companyJobPostDto.AdDuration);
            if (pricingPlan == null)
            {
                _logger.LogWarning($"UserId: {userId} selected an invalid pricing plan.");
                return BadRequest("Niste odabrali ispravan plan paketa oglasa.");
            }
               
            if (user.Credits < pricingPlan.PriceInCredits)
            {
                _logger.LogWarning($"UserId: {userId} does not have enough credits to create a job post.");
                return BadRequest("Nemate dovoljno kredita za objavu odabranog tipa oglasa. Molimo Vas da dopunite kredite ili odaberete neki drugi paket oglasa");
            }
            companyJobPostDto.SubmittingUserId = userId;
            companyJobPostDto.PricingPlanId = pricingPlan.Id;
            var sanitizer = new HtmlSanitizer();
            string sanitizedJobDescription = sanitizer.Sanitize(companyJobPostDto.JobDescription);
            string sanitizedBenefits = sanitizer.Sanitize(companyJobPostDto.Benefits);
            string sanitizedWorkEnv = sanitizer.Sanitize(companyJobPostDto.WorkEnvironmentDescription);
            companyJobPostDto.JobDescription = sanitizedJobDescription;
            companyJobPostDto.WorkEnvironmentDescription = sanitizedWorkEnv;
            companyJobPostDto.Benefits = sanitizedBenefits;

            try
            {
                var newItem = await _jobPostService.CreateCompanyJobPostAsync(companyJobPostDto);
                newItem.CurrentUserCredits = user.Credits;
                return Ok(newItem);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to create job post for UserId: {userId}. Error: {ex.Message}");
                return StatusCode(500, "Došlo je do greške prilikom kreiranja oglasa. Pokušajte ponovo kasnije.");
            }
        }

        [HttpPost("uploadLogo/{id}")]
        public async Task<IActionResult> UploadAdLogo(int id, IFormFile photo)
        {
            if(photo == null)
            {
                return BadRequest("Logo nije priložen.");
            }
            if (!FileHelper.IsValidImage(photo))
            {
                return BadRequest("Nevažeći format slike. Dozvoljeni formati: JPG, PNG, GIF, BMP, WEBP.");
            }
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

            var sanitizer = new HtmlSanitizer();
            string sanitizedDescription = sanitizer.Sanitize(companyJobPostDto.JobDescription);
            companyJob.JobDescription = sanitizedDescription;
            companyJob.CityId = companyJobPostDto.CityId;
            companyJob.JobCategoryId = companyJobPostDto.JobCategoryId;
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
            var sanitizer = new HtmlSanitizer();
            string sanitizedBenefits = sanitizer.Sanitize(companyJobPostDto.Benefits);
            string sanitizedWorkEnv = sanitizer.Sanitize(companyJobPostDto.WorkEnvironmentDescription);

            companyJob.Benefits = sanitizedBenefits;
            companyJob.WorkEnvironmentDescription = sanitizedWorkEnv;
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
