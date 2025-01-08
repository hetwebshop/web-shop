using API.Data;
using API.DTOs;
using API.Entities.Applications;
using API.Extensions;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    public class ApplicationsController : BaseController
    {
        private IUserApplicationsRepository userApplicationsRepository;
        private readonly IUnitOfWork _uow;
        private readonly IBlobStorageService _blobStorageService;

        public ApplicationsController(IUserApplicationsRepository userApplicationsRepository, IUnitOfWork uow, IBlobStorageService blobStorageService)
        {
            this.userApplicationsRepository = userApplicationsRepository;
            _uow = uow;
            _blobStorageService = blobStorageService;   
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
            if (!string.IsNullOrEmpty(req.MeetingDateTime))
            {
                // Ensure parsing treats the input as UTC
                DateTime dateTimeUtc = DateTime.Parse(req.MeetingDateTime, null, System.Globalization.DateTimeStyles.RoundtripKind);

                // Ensure the DateTimeKind is explicitly UTC
                dateTimeUtc = DateTime.SpecifyKind(dateTimeUtc, DateTimeKind.Utc);

                req.MeetingDateTimeDateType = dateTimeUtc;
            }
            var updatedApplication = await userApplicationsRepository.UpdateUserApplicationStatusAsync(req);
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
            return Ok(areApplicationsRejected);
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
