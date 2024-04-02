using API.DTOs;
using API.Extensions;
using API.Helpers;
using API.Mappers;
using API.PaginationEntities;
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

        [HttpGet("ads")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAds([FromQuery] AdsPaginationParameters adsParameters)
        {
            var jobPosts = await _jobPostService.GetJobPostsAsync(adsParameters);
            var pagedResponse = jobPosts.ToPagedResponse();
            return Ok(pagedResponse);
        }

        [HttpGet("my-ads")]
        public async Task<IActionResult> GetMyAds()
        {
            var currentUserId = HttpContext.User.GetUserId(); ;

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

            userJobPostDto.SubmittingUserId = HttpContext.User.GetUserId(); ;
            var newItem = await _jobPostService.CreateUserJobPostAsync(userJobPostDto);
            return Ok(newItem);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUserJobPost(int id, [FromBody]UserJobPostDto userJobPostDto)
        {
            userJobPostDto.SubmittingUserId = HttpContext.User.GetUserId(); ;
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
