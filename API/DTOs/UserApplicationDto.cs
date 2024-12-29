using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace API.DTOs
{
    public class UserApplicationDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public int CityId { get; set; }
        public string City { get; set; }
        public int SubmittingUserId { get; set; }
        public IFormFile CvFile { get; set; }
        public string CvFilePath { get; set; }
        public string CvFileName { get; set; }
        public bool? IsUserProfileCvFileSubmitted { get; set; }
        public int CompanyJobPostId { get; set; }
        public string CoverLetter { get; set; }
        public ApplicationStatus ApplicationStatusId { get; set; }
        public DateTime ApplicationCreatedAt { get; set; }
        public int? EducationLevelId { get; set; }
        public string EducationLevel { get; set; }
        public int? YearsOfExperience { get; set; }

        public string OnlineMeetingLink { get; set; }
        public bool? IsOnlineMeeting { get; set; }
        public string Position { get; set; }
        public DateTime? MeetingDateTime { get; set; }

        public int? AIMatchingResult { get; set; }
        public int? AIMatchingExperience { get; set; }
        public int? AIMatchingSkills { get; set; }
        public int? AIMatchingEducationLevel { get; set; }
        public string AIMatchingDescription { get; set; }

        public List<UserEducationDto> Educations { get; set; }
        public List<UserPreviousCompaniesDto> PreviousCompanies { get; set; }
    }


    public enum ApplicationStatus
    {
        WaitingForResponse,
        Accepted,
        Rejected
    }
}
