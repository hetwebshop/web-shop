using System;

namespace API.Entities.Chat
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }  // Link to conversation
        public string Message { get; set; }  // Message text
        public int FromUserId { get; set; }  // Sender
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; }

        public Conversation Conversation { get; set; }
        public User FromUser { get; set; }
    }
}
