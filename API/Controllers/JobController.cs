using API.DTOs;
using API.Extensions;
using API.Helpers;
using API.Mappers;
using API.PaginationEntities;
using API.Services.UserOfferServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
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
        public async Task<IActionResult> GetMyAds([FromQuery] AdsPaginationParameters adsParameters)
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
            if (userJob != null && userJob.SubmittingUserId != currentUserId)
                return Unauthorized("Nemate pravo pristupa ovom oglasu");
            return Ok(userJob);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUserJobPost([FromForm]UserJobPostDto userJobPostDto)
        {

            userJobPostDto.SubmittingUserId = HttpContext.User.GetUserId();
            if(userJobPostDto.CvFile != null)
            {
                var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
                if (!Directory.Exists(uploadsDir))
                {
                    Directory.CreateDirectory(uploadsDir);
                }

                var filePath = Path.Combine(uploadsDir, userJobPostDto.CvFile.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await userJobPostDto.CvFile.CopyToAsync(stream);
                }
                userJobPostDto.CvFilePath = filePath;
            }

            var newItem = await _jobPostService.CreateUserJobPostAsync(userJobPostDto);
            return Ok(newItem);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUserJobPost(int id, [FromForm] UserJobPostDto userJobPostDto)
        {
            userJobPostDto.SubmittingUserId = HttpContext.User.GetUserId();
            var updatedItem = await _jobPostService.UpdateUserJobPostAsync(userJobPostDto);
            return Ok(updatedItem);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUserJobPost(int id)
        {
            var userId = HttpContext.User.GetUserId();
            var deleted = await _jobPostService.DeleteUserJobPostByIdAsync(userId, id);
            return Ok(deleted);
        }

        [HttpPatch("close/{id}")]
        public async Task<IActionResult> CloseUserJobPost(int id)
        {
            var userId = HttpContext.User.GetUserId();
            var closed = await _jobPostService.CloseUserJobPostByIdAsync(userId, id);
            return Ok(closed);
        }

        [HttpPatch("reactivate/{id}")]
        public async Task<IActionResult> ReactivateUserJobPost(int id)
        {
            var userId = HttpContext.User.GetUserId();
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
    }
}
