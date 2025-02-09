using System;

namespace API.Entities.JobPost
{
    public class ContactUserRequest
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public int UserJobPostId { get; set; }
        public UserJobPost UserJobPost { get; set; }
        public int FromUserId { get; set; }
        public User FromUser { get; set; }
        public int ToUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CompanyName { get; set; }
    }
}
