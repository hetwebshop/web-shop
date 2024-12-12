using API.Entities.JobPost;
using System;

namespace API.Entities
{
    public class ApplicantPreviousCompanies
    {
        public int Id { get; set; }
        public int UserJobPostId { get; set; }
        public UserJobPost UserJobPost { get; set; }
        public string CompanyName { get; set; }
        public int StartYear { get; set; }
        public int? EndYear { get; set; }
        public string Position { get; set; }
        public string? Description { get; set; }
    }
}
