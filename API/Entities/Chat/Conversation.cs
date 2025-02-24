using API.Entities.JobPost;
using System.Collections.Generic;
using System;

namespace API.Entities.Chat
{
    public class Conversation
    {
        public int Id { get; set; }  // Unique conversation identifier
        public int FromUserId { get; set; }  // Sender
        public int ToUserId { get; set; }  // Receiver
        public int? UserJobPostId { get; set; }  // Job post by user
        public int? CompanyJobPostId { get; set; }  // Job post by company
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User FromUser { get; set; }
        public User ToUser { get; set; }
        public UserJobPost? UserJobPost { get; set; }
        public CompanyJobPost.CompanyJobPost? CompanyJobPost { get; set; }

        public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>(); // List of messages
    }
}
