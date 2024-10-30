using API.Entities;
using API.Entities.JobPost;
using API.Helpers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace API.DTOs
{
    public class UserJobPostDto
    {
        public int Id { get; set; }
        public string Position { get; set; }
        public string Biography { get; set; }
        public string ApplicantFirstName { get; set; }
        public string ApplicantLastName { get; set; }
        public string ApplicantEmail { get; set; }
        public string ApplicantPhoneNumber { get; set; }
        public DateTime ApplicantDateOfBirth { get; set; }
        public string ApplicantGender { get; set; }
        public decimal? Price { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int JobTypeId { get; set; }
        public string JobType { get; set; }
        public int JobCategoryId { get; set; }
        public string JobCategory { get; set; }
        public int JobPostStatusId { get; set; }
        public string JobPostStatus { get; set; }
        public int CityId { get; set; }
        public string City { get; set; }
        public int SubmittingUserId { get; set; }
        public int AdvertisementTypeId { get; set; }
        public IFormFile CvFile { get; set; }
        public string CvFilePath { get; set; }
        public int AdDuration { get; set; }
        public DateTime AdStartDate { get; set; }
        public DateTime AdEndDate { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string AdTitle { get; set; }
        public string AdAdditionalDescription { get; set; }

        public List<ApplicantEducationDto> ApplicantEducations { get; set; }
    }
}
