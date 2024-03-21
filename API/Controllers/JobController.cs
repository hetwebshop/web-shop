using API.DTOs;
using API.Services.UserOfferServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace API.Controllers
{
    //[Authorize]
    public class JobController : BaseController
    {
        private readonly IUserJobPostService _jobPostService;

        public JobController(IUserJobPostService jobPostService)
        {
            _jobPostService = jobPostService;
        }

        [HttpGet("alljobposts")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllJobPosts()
        {
            var allJobPosts = await _jobPostService.GetAllUserJobPostsAsync();
            return Ok(allJobPosts);
        }

        [HttpGet("my-ads")]
        public async Task<IActionResult> GetMyAds()
        {
            var currentUserId = (int)HttpContext.Items["SubmittingUserId"];

            var myAds = await _jobPostService.GetMyAdsAsync(currentUserId);
            return Ok(myAds);
        }

        [HttpGet("user-job/{id}")]
        public async Task<IActionResult> GetUserJobById(int id)
        {
            var userJob = await _jobPostService.GetUserJobPostByIdAsync(id);
            return Ok(userJob);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUserJobPost([FromBody]UserJobPostDto userJobPostDto)
        {

            userJobPostDto.SubmittingUserId = (int)HttpContext.Items["SubmittingUserId"];
            var newItem = await _jobPostService.CreateUserJobPostAsync(userJobPostDto);
            return Ok(newItem);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUserJobPost(int id, [FromBody]UserJobPostDto userJobPostDto)
        {
            userJobPostDto.SubmittingUserId = (int)HttpContext.Items["SubmittingUserId"];
            var updatedItem = await _jobPostService.UpdateUserJobPostAsync(userJobPostDto);
            return Ok(updatedItem);
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
    }
}
