using System;

namespace API.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }  // Foreign key to your User table
        public string Token { get; set; } //this is refresh token
        public string DeviceId { get; set; } // Unique identifier for each device
        public string UserAgent { get; set; } // Browser or app details
        public string IPAddress { get; set; } // IP address of the request
        public DateTime ExpiryDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
