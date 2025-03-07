using API.DTOs;
using API.Helpers;
using System;
using System.Collections.Generic;

namespace API.Entities.Applications
{
    public class UserApplication
    {
        public int Id { get; set; }
        public int SubmittingUserId { get; set; }
        public User User { get; set; }
        public int CompanyJobPostId { get; set; }
        public API.Entities.CompanyJobPost.CompanyJobPost CompanyJobPost { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }

        public string Biography { get; set; }

        public ApplicationStatus ApplicationStatusId { get; set; }

        public string CoverLetter { get; set; }

        public Gender Gender { get; set; }
        public int CityId { get; set; }
        public City City { get; set; }

        public string CvFilePath { get; set; }
        public string CvFileName { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string Feedback { get; set; }
        public bool? IsOnlineMeeting { get; set; }
        public string OnlineMeetingLink { get; set; }
        public DateTime? MeetingDateTime { get; set; }

        public int? EducationLevelId { get; set; }
        public EducationLevel? EducationLevel { get; set; }
        
        public int? YearsOfExperience { get; set; }

        public ICollection<UserApplicationEducation> Educations { get; set; }
        public ICollection<UserApplicationPreviousCompanies> PreviousCompanies { get; set; }

        public double? AIMatchingResult { get; set; }
        public double? AIMatchingExperience { get; set; }
        public double? AIMatchingSkills { get; set; }
        public double? AIMatchingEducationLevel { get; set; }
        public string AIMatchingDescription { get; set; }
    }

}
