using API.Data;
using API.DTOs;
using API.Entities;
using API.Entities.Applications;
using API.Extensions;
using API.Helpers;
using API.Migrations;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    public class ApplicationsController : BaseController
    {
        private IUserApplicationsRepository userApplicationsRepository;
        private readonly IUnitOfWork _uow;
        private readonly IBlobStorageService _blobStorageService;
        private readonly string UIBaseUrl;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly ISendNotificationsQueueClient _sendNotificationsQueueClient;
        private readonly DataContext _dbContext;
        private readonly ILogger<ApplicationsController> _logger;

        public ApplicationsController(IUserApplicationsRepository userApplicationsRepository, IUnitOfWork uow, IBlobStorageService blobStorageService, IConfiguration configuration, IEmailService emailService, ISendNotificationsQueueClient sendNotificationsQueueClient, DataContext dbContext, ILogger<ApplicationsController> logger)
        {
            this.userApplicationsRepository = userApplicationsRepository;
            _uow = uow;
            _blobStorageService = blobStorageService;
            _configuration = configuration;
            UIBaseUrl = configuration.GetSection("UIBaseUrl").Value;
            _emailService = emailService;
            _sendNotificationsQueueClient = sendNotificationsQueueClient;
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet("userapplications")]
        public async Task<IActionResult> GetAllUserApplications()
        {
            var currentUserId = HttpContext.User.GetUserId();
            var userApplications = await userApplicationsRepository.GetAllUserApplicationsAsync(currentUserId);
            var userApplicationsTableData = new List<UserApplicationTableDataDto>();
            foreach(var application in userApplications)
            {
                var tableData = new UserApplicationTableDataDto()
                {
                    JobPosition = application.CompanyJobPost.Position,
                    CompanyCity = application.CompanyJobPost?.City?.Name,
                    CreatedAt = application.CreatedAt,
                    CompanyName = application.CompanyJobPost?.User?.Company?.CompanyName,
                    CompanyJobPostId = application.CompanyJobPostId,
                    ApplicationStatusId = application.ApplicationStatusId,
                    Email = application.Email,
                    Feedback = application.Feedback,
                    MeetingDateTime = application.MeetingDateTime,
                    Id = application.Id
                };
                userApplicationsTableData.Add(tableData);
            }
            return Ok(userApplicationsTableData);
        }

        [HttpGet("userapplication/{id}")]
        public async Task<IActionResult> GetUserApplicationById(int id)
        {
            var currentUserId = HttpContext.User.GetUserId();
            var userApplication = await userApplicationsRepository.GetUserApplicationByIdAsync(id);
            var user = await _uow.UserRepository.GetUserByIdAsync(currentUserId);
            if (user == null || !user.IsCompany)
            {
                return Forbid("Nemate pravo pristupa.");
            }
            if (userApplication == null)
                return NotFound("Oglas ne postoji!");
            if (user.Id != userApplication.CompanyJobPost.SubmittingUserId)
                return Forbid("Nemate pravo pristupa");

            var userApplicationDto = ConvertToDto(userApplication);
            return Ok(userApplicationDto);
        }

        [HttpGet("cvfile")]
        public async Task<IActionResult> GetCVFile([FromQuery] int userApplicationId)
        {
            var userId = HttpContext.User.GetUserId();
            if (userId == null)
            {
                return Forbid("Nemate pravo pristupa.");
            }
            var user = await _uow.UserRepository.GetUserByIdAsync(userId);
            if (user == null || !user.IsCompany)
            {
                return Forbid("Nemate pravo pristupa.");
            }
            var userApplication = await userApplicationsRepository.GetUserApplicationByIdAsync(userApplicationId);
            if (userApplication == null)
                return NotFound("Oglas ne postoji!");
            if (user.Id != userApplication.CompanyJobPost.SubmittingUserId)
                return Forbid("Nemate pravo pristupa");
            var fileName = Path.GetFileName(userApplication.CvFilePath);
            var fileDto = await _blobStorageService.GetFileAsync(fileName);

            if (fileDto.FileContent == null)
            {
                return NotFound("Cv nije pronađen.");
            }

            return File(fileDto.FileContent, fileDto.MimeType, fileName);
        }

        [HttpPatch("updateapplicationstatus/{applicationId}")]
        public async Task<IActionResult> UpdateApplicationStatus(int applicationId, [FromBody] UpdateApplicationStatusRequest req)
        {
            var userId = HttpContext.User.GetUserId();
            if (userId == null)
            {
                return Forbid("Nemate pravo pristupa.");
            }
            var user = await _uow.UserRepository.GetUserByIdAsync(userId);
            if (user == null || !user.IsCompany)
            {
                return Forbid("Nemate pravo pristupa.");
            }
            var userApplication = await userApplicationsRepository.GetUserApplicationByIdAsync(applicationId);
            if (userApplication == null)
                return NotFound("Oglas ne postoji!");
            if (user.Id != userApplication.CompanyJobPost.SubmittingUserId)
                return Forbid("Nemate pravo pristupa");
            req.UserApplicationId = applicationId;
            req.MeetingDateTimeDateType = req.MeetingDateTime;
            var updatedApplication = await userApplicationsRepository.UpdateUserApplicationStatusAsync(req);
            var jobPostNotificationMessage = new ApplicantStatusUpdated
            {
                UserApplicationIds = new List<int> { userApplication.Id }
            };
            await _sendNotificationsQueueClient.SendMessageToUserOnUpdateApplicationStatusAsync(jobPostNotificationMessage);

            var userApplicationDto = ConvertToDto(updatedApplication);
            return Ok(userApplicationDto);
        }

        [HttpPatch("rejectselectedcandidates")]
        public async Task<IActionResult> RejectSelectedCandidates([FromBody] RejectSelectedCandidatesRequest req)
        {
            var userId = HttpContext.User.GetUserId();
            if (userId == null)
            {
                return Forbid("Nemate pravo pristupa.");
            }
            var user = await _uow.UserRepository.GetUserByIdAsync(userId);
            if (user == null || !user.IsCompany)
            {
                return Forbid("Nemate pravo pristupa.");
            }
            foreach(var userApplicationId in req.Candidates)
            {
                var userApplication = await userApplicationsRepository.GetUserApplicationByIdAsync(userApplicationId);
                if (userApplication == null)
                    return NotFound("Oglas ne postoji!");
                if (user.Id != userApplication.CompanyJobPost.SubmittingUserId)
                    return Forbid("Nemate pravo pristupa");
            }

            var areApplicationsRejected = await userApplicationsRepository.RejectSelectedCandidatesAsync(req);
            var jobPostNotificationMessage = new ApplicantStatusUpdated
            {
                UserApplicationIds = req.Candidates
            };
            await _sendNotificationsQueueClient.SendMessageToUserOnUpdateApplicationStatusAsync(jobPostNotificationMessage);
            return Ok(areApplicationsRejected);
        }

        [HttpPost("addcomment")]
        public async Task<IActionResult> AddCandidateComment([FromBody] AddCandidateCommentRequest req)
        {
            try
            {
                var userId = HttpContext.User.GetUserId();
                var user = await _uow.UserRepository.GetUserByIdAsync(userId);
                if (user == null || user.CompanyId == null)
                {
                    return Unauthorized("Nemate pravo pristupa.");
                }

                var companyJobPost = await _dbContext.CompanyJobPosts.FirstOrDefaultAsync(r => r.Id == req.CompanyJobPostId);
                if (companyJobPost == null)
                {
                    return BadRequest("Oglas ne postoji.");
                }

                var userApplication = await _dbContext.UserApplications.FirstOrDefaultAsync(r => r.Id == req.UserApplicationId);
                if (userApplication == null)
                {
                    return BadRequest("Aplikacija ne postoji");
                }

                if (userApplication.CompanyJobPostId != companyJobPost.Id && companyJobPost.SubmittingUserId != userId)
                {
                    _logger.LogWarning($"Unauthorized access attempt by user with ID: {userId} for JobPostId: {companyJobPost.Id}");
                    return Unauthorized("Nemate pravo pristupa");
                }

                var candidateComment = new CandidateComment()
                {
                    CandidateId = req.CandidateId,
                    Comment = req.Comment,
                    CompanyUserId = userId,
                    Date = DateTime.UtcNow,
                    UserApplicationId = req.UserApplicationId
                };

                await _dbContext.AddAsync(candidateComment);
                await _dbContext.SaveChangesAsync();

                return Ok(candidateComment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a comment.");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        [HttpGet("companycandidatecomments/{candidateId}")]
        public async Task<IActionResult> GetAllCandidateCommentsByCompany(int candidateId)
        {
            try
            {
                var userId = HttpContext.User.GetUserId();
                var user = await _uow.UserRepository.GetUserByIdAsync(userId);
                if (user == null || user.CompanyId == null)
                {
                    return Unauthorized("Nemate pravo pristupa.");
                }

                var candidateComments = await _dbContext.CandidateComments
                    .Where(r => r.CandidateId == candidateId && r.CompanyUserId == userId).OrderBy(r => r.Date)
                    .ToListAsync();

                return Ok(candidateComments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving comments for CandidateId: {candidateId}.");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        [HttpDelete("deletecomment/{id}")]
        public async Task<IActionResult> DeleteCandidateComment(int id)
        {
            try
            {
                var userId = HttpContext.User.GetUserId();
                var user = await _uow.UserRepository.GetUserByIdAsync(userId);
                if (user == null || user.CompanyId == null)
                {
                    return Unauthorized("Nemate pravo pristupa.");
                }

                var candidateComment = await _dbContext.CandidateComments
                    .FirstOrDefaultAsync(r => r.CompanyUserId == userId && r.Id == id); 

                if(candidateComment != null)
                {
                   _dbContext.Remove(candidateComment);
                    await _dbContext.SaveChangesAsync();
                    return Ok(true);
                }

                return Ok(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting comment with id: {id}.");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        [HttpPut("updatecomment/{id}")]
        public async Task<IActionResult> UpdateCandidateComment(int id, [FromBody] UpdateCandidateCommentRequest req)
        {
            try
            {
                var userId = HttpContext.User.GetUserId();
                var user = await _uow.UserRepository.GetUserByIdAsync(userId);
                if (user == null || user.CompanyId == null)
                {
                    return Unauthorized("Nemate pravo pristupa.");
                }

                var candidateComment = await _dbContext.CandidateComments
                    .FirstOrDefaultAsync(r => r.CompanyUserId == userId && r.Id == id);

                if (candidateComment != null)
                {
                    candidateComment.Comment = req.Comment;
                    await _dbContext.SaveChangesAsync();
                    return Ok(true);
                }

                return Ok(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating comment with id: {id}.");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        public static UserApplicationDto ConvertToDto(UserApplication userApplication)
        {
            return new UserApplicationDto
            {
                Id = userApplication.Id,
                FirstName = userApplication.FirstName,
                LastName = userApplication.LastName,
                ApplicationStatusId = userApplication.ApplicationStatusId,
                City = userApplication.City?.Name,
                CoverLetter = userApplication.CoverLetter,
                CvFilePath = userApplication.CvFilePath,
                CvFileName = userApplication.CvFileName,
                ApplicationCreatedAt = userApplication.CreatedAt,
                OnlineMeetingLink = userApplication.OnlineMeetingLink,
                IsOnlineMeeting = userApplication.IsOnlineMeeting,
                Position = userApplication.CompanyJobPost?.Position,
                MeetingDateTime = userApplication.MeetingDateTime,
                Biography = userApplication.Biography,
                PreviousCompanies = userApplication.PreviousCompanies?.Select(userPreviousCompany => new UserPreviousCompaniesDto
                {
                    CompanyName = userPreviousCompany.CompanyName,
                    Position = userPreviousCompany.Position,
                    Description = userPreviousCompany.Description,
                    StartYear = userPreviousCompany.StartYear,
                    EndYear = userPreviousCompany.EndYear,
                    UserCompanyId = userPreviousCompany.Id
                }).ToList() ?? new List<UserPreviousCompaniesDto>(),
                Educations = userApplication.Educations?.Select(userEducation => new UserEducationDto
                {
                    University = userEducation.University,
                    EducationEndYear = userEducation.EducationEndYear,
                    EducationStartYear = userEducation.EducationStartYear,
                    Degree = userEducation.Degree,
                    FieldOfStudy = userEducation.FieldOfStudy,
                    UserEducationId = userEducation.Id,
                    InstitutionName = userEducation.InstitutionName
                }).ToList() ?? new List<UserEducationDto>(),
                DateOfBirth = userApplication.DateOfBirth,
                Email = userApplication.Email,
                PhoneNumber = userApplication.PhoneNumber,
                Gender = userApplication.Gender.ToString(),
                YearsOfExperience = userApplication.YearsOfExperience,
                EducationLevel = userApplication.EducationLevel?.Name,
                EducationLevelId = userApplication.EducationLevelId,
                AIMatchingDescription = userApplication.AIMatchingDescription,
                AIMatchingEducationLevel = userApplication.AIMatchingEducationLevel,
                AIMatchingExperience = userApplication.AIMatchingExperience,
                AIMatchingResult = userApplication.AIMatchingResult,
                AIMatchingSkills = userApplication.AIMatchingSkills
            };
        }
    }
}
