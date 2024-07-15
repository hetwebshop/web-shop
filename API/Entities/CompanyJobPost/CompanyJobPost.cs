using API.Entities.JobPost;
using System;

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
    }
}
