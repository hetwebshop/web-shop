using API.Entities.Applications;
using API.Entities.JobPost;
using System;
using System.Collections.Generic;

namespace API.Entities.CompanyJobPost
{
    public class CompanyJobPost
    {
        public int Id { get; set; }
        public string JobDescription { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int SubmittingUserId { get; set; }
        public User User { get; set; }
        public int JobTypeId { get; set; }
        public JobType JobType { get; set; }
        public int JobCategoryId { get; set; }
        public JobCategory JobCategory { get; set; }
        public int JobPostStatusId { get; set; }
        public JobPostStatus JobPostStatus { get; set; }
        public int CityId { get; set; } = 1;
        public City City { get; set; }
        public DateTime AdStartDate { get; set; }
        public DateTime AdEndDate { get; set; }
        public int AdDuration { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string AdName { get; set; }
        public string Position { get; set; }
        public string EmailForReceivingApplications { get; set; }
        public PricingPlan PricingPlan { get; set; }
        public int PricingPlanId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyCity { get; set; }
        public int EmploymentTypeId { get; set; }
        public EmploymentType EmploymentType { get; set; }
        public string Benefits { get; set; }
        public string WorkEnvironmentDescription { get; set; }
        public string RequiredSkills { get; set; }
        public int? EducationLevelId { get; set; }
        public EducationLevel? EducationLevel { get; set; }
        public string Certifications { get; set; }
        public int? RequiredExperience { get; set; }
        public string HowToApply { get; set; }
        public string DocumentsRequired { get; set; }
        public int? MinSalary { get; set; }
        public int? MaxSalary { get; set; }

        public ICollection<UserApplication> UserApplications { get; set; }
    }
}
