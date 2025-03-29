using System;

namespace API.DTOs
{
    public class UserJobOffersTableData
    {
        public int Id { get; set; }
        public int? UserJobPostId { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public string JobPosition { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Message { get; set; }
        public string Phone { get; set; }
        public string Subject { get; set; }
        public int? CompanyJobPostId { get; set; }
    }
}
