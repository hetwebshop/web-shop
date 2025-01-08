using API.Data;
using API.Data.ICompanyJobPostRepository;
using API.DTOs;
using API.Extensions;
using API.Mappers;
using API.PaginationEntities;
using API.Services.CompanyJobPostServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public CompanyJobController(ICompanyJobPostService jobPostService, IUnitOfWork uow, ICompanyJobPostRepository companyJobPostRepository, IUserApplicationsRepository userApplicationsRepository)
        {
            _jobPostService = jobPostService;
            _uow = uow;
            _jobPostRepository = companyJobPostRepository;
            _userApplicationsRepository = userApplicationsRepository;
        }

        [HttpPost("ads")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAds([FromQuery] AdsPaginationParameters adsParameters)
        {
            var jobPosts = await _jobPostService.GetJobPostsAsync(adsParameters);
            var pagedResponse = jobPosts.ToPagedResponse();
            var currentUserId = HttpContext.User.GetUserId();
            if(currentUserId != null && currentUserId != 0)
            {
                foreach(var item in pagedResponse.Items)
                {
                    if (item.UsersThatAppliedOnJobPost.Any() && item.UsersThatAppliedOnJobPost.Contains(currentUserId))
                        item.CanCurrentUserApplyOnAd = false;
                }
            }
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
        [AllowAnonymous]
        public async Task<IActionResult> GetCompanyJobById(int id)
        {
            var companyJob = await _jobPostService.GetCompanyJobPostByIdAsync(id);
            var currentUserId = HttpContext.User.GetUserId();
            if (currentUserId != null && currentUserId != 0)
            {
                companyJob.CanCurrentUserApplyOnAd = companyJob.UsersThatAppliedOnJobPost.Contains(currentUserId) ? false : true;
            }
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
            return Ok(companyJobs);
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
            if (companyJobPost.JobPostStatusId != 1 || companyJobPost.AdEndDate < DateTime.Now)
                return BadRequest("Oglas je istekao");

            var userApplicationsForJobPost = await _userApplicationsRepository.GetApplicationsForSpecificCompanyJobPost(jobId);
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
                    AIMatchingExperience = application.AIMatchingExperience
                };
                candidatesTableData.Add(tableData);
            }
            return Ok(candidatesTableData);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCompanyJobPost([FromBody] CompanyJobPostDto companyJobPostDto)
        {
            companyJobPostDto.SubmittingUserId = HttpContext.User.GetUserId();
            var newItem = await _jobPostService.CreateCompanyJobPostAsync(companyJobPostDto);
            return Ok(newItem);
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
            var updatedItem = await _jobPostService.UpdateCompanyJobPostAsync(companyJob);
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
