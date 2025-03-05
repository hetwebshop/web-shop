using API.Entities.Applications;
using API.Entities.JobPost;
using API.Helpers;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace API.Entities
{
    public class User : IdentityUser<int>
    {
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public DateTime LastActive { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhotoUrl { get; set; }
        public int CityId { get; set; }
        public City City { get; set; }
        public int? JobTypeId { get; set; }
        public JobType JobType { get; set; }
        public int? JobCategoryId { get; set; }
        public JobCategory JobCategory { get; set; }
        public string Position { get; set; }
        public string Biography { get; set; }
        public string CvFilePath { get; set; }
        public string CvFileName { get; set; }
        public double Credits { get; set; }

        public bool IsCompany { get; set; }
        public int? CompanyId { get; set; }
        public Company? Company { get; set; }

        public bool IsApproved { get; set; }

        public int? EducationLevelId { get; set; }
        public EducationLevel? EducationLevel { get; set; }

        public int? EmploymentTypeId { get; set; }
        public EmploymentType? EmploymentType { get; set; }

        public int? YearsOfExperience { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<UserJobPost> UserJobPosts { get; set; }
        public ICollection<UserEducation> UserEducations { get; set; }

        public ICollection<UserPreviousCompanies> UserPreviousCompanies { get; set; }


        public int? EmploymentStatusId { get; set; }
        public EmploymentStatus? EmploymentStatus { get; set; }

        public ICollection<CompanyJobPost.CompanyJobPost> CompanyJobPosts { get; set; }

        public ICollection<UserApplication> UserApplications { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        public string Coverletter { get; set; }
    }
}
