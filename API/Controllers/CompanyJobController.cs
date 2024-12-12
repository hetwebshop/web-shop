using API.Data;
using API.Data.ICompanyJobPostRepository;
using API.DTOs;
using API.Extensions;
using API.Mappers;
using API.PaginationEntities;
using API.Services.CompanyJobPostServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class CompanyJobController : BaseController
    {
        private readonly ICompanyJobPostService _jobPostService;
        private readonly IUnitOfWork _uow;
        private readonly ICompanyJobPostRepository _jobPostRepository;

        public CompanyJobController(ICompanyJobPostService jobPostService, IUnitOfWork uow, ICompanyJobPostRepository companyJobPostRepository)
        {
            _jobPostService = jobPostService;
            _uow = uow;
            _jobPostRepository = companyJobPostRepository;
        }

        [HttpPost("ads")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAds([FromQuery] AdsPaginationParameters adsParameters)
        {
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
