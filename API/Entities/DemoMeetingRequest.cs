using System;

namespace API.Entities
{
    public class DemoMeetingRequest
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Company { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsProcessed { get; set; } = false;
    }
}
