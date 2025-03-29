using API.Entities.Applications;
using API.Entities.Chat;

namespace API.DTOs
{
    public class UserApplicationConversationDto
    {
        public UserApplication UserApplication { get; set; }
        public int? ConversationId { get; set; }
    }
}
