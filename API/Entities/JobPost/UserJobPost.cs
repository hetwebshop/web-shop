using API.DTOs;
using API.Helpers;
using System;
using System.Collections.Generic;

namespace API.Entities.JobPost
{
    public class UserJobPost
    {
        public int Id { get; set; }
        public string Position { get; set; }
        public string Biography { get; set; }
        public string ApplicantFirstName { get; set; }
        public string ApplicantLastName { get; set; }
        public string ApplicantEmail { get; set; }
        public string ApplicantPhoneNumber { get; set; }
        public DateTime? ApplicantDateOfBirth { get; set; }
        public Gender? ApplicantGender { get; set; }
        public decimal? Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int SubmittingUserId { get; set; }
        public User User { get; set; }
        public int? JobTypeId { get; set; }
        public JobType? JobType { get; set; }
        public int? JobCategoryId { get; set; }
        public JobCategory? JobCategory { get; set; }
        public int JobPostStatusId { get; set; }
        public JobPostStatus JobPostStatus { get; set; }
        public int CityId { get; set; } = 1;
        public City City { get; set; } 
        public int AdvertisementTypeId { get; set; }
        public AdvertisementType AdvertisementType { get; set; }
        public string CvFilePath { get; set; }
        public string CvFileName { get; set; }
        public DateTime AdStartDate { get; set; }
        public DateTime AdEndDate { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string AdTitle { get; set; }
        public string AdAdditionalDescription { get; set; }
        public PricingPlan PricingPlan { get; set; }
        public int PricingPlanId { get; set; }
        public DateTime RefreshDateTime { get; set; }
        public int? RefreshIntervalInDays { get; set; }

        public int? EducationLevelId { get; set; }
        public EducationLevel? EducationLevel { get; set; }

        public int? EmploymentTypeId { get; set; }
        public EmploymentType? EmploymentType { get; set; }

        public int? YearsOfExperience { get; set; }

        public int? EmploymentStatusId { get; set; }
        public EmploymentStatus? EmploymentStatus { get; set; }

        public ICollection<ApplicantEducation> ApplicantEducations { get; set; }
        public ICollection<ApplicantPreviousCompanies> ApplicantPreviousCompanies { get; set; }
    }
}
