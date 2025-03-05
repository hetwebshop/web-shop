using API.Entities.CompanyJobPost;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace API.DTOs
{
    public class CompanyJobPostDto
    {
        public int Id { get; set; }
        public string JobDescription { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int? JobTypeId { get; set; }
        public string JobType { get; set; }
        public int JobCategoryId { get; set; }
        public string JobCategory { get; set; }
        public int JobPostStatusId { get; set; }
        public string JobPostStatus { get; set; }
        public int CityId { get; set; }
        public string City { get; set; }
        public int SubmittingUserId { get; set; }
        public int AdDuration { get; set; }
        public string PricingPlanName { get; set; }
        public int? PricingPlanId { get; set; }
        public DateTime AdStartDate { get; set; }
        public DateTime AdEndDate { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string Position { get; set; }
        public string EmailForReceivingApplications { get; set; }
        public int EmploymentTypeId { get; set; }
        public string EmploymentType { get; set; }
        public string Benefits { get; set; }
        public string WorkEnvironmentDescription { get; set; }
        public string RequiredSkills { get; set; }
        public int? EducationLevelId { get; set; }
        public string EducationLevel { get; set; }
        public string Certifications { get; set; }
        public int? RequiredExperience { get; set; }
        public string HowToApply { get; set; }
        public string DocumentsRequired { get; set; }
        public int? MinSalary { get; set; }
        public int? MaxSalary { get; set; }
        public List<int>? UsersThatAppliedOnJobPost { get; set; }
        public bool? CanCurrentUserApplyOnAd { get; set; } = true;
        public DateTime? RefreshDateTime { get; set; }
        public int? RefreshIntervalInDays { get; set; }
        public string? PhotoUrl { get; set; }
        public string CompanyName { get; set; }
        public IFormFile? Logo { get; set; }
        //public bool IsAiAnalysisIncluded { get; set; }
        public double? AiAnalysisProgress { get; set; }
        public DateTime? AiAnalysisStartedOn { get; set; } //if we have base ad, and user triggered analysis for all candidates, we need this to track if there is an issue
        public double CurrentUserCredits { get; set; }
        public AiAnalysisStatus? AiAnalysisStatus { get; set; }
    }
}
