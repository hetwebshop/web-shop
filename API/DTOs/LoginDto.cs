using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class LoginDto
    {
        [Required]
        public string UserNameOrEmail { get; set; }

        [Required]
        public string Password { get; set; }

        public string UserAgent { get; set; }  // The User-Agent string from the HTTP request
        public string IPAddress { get; set; }  // The IP address from the HTTP request
        public string DeviceId { get; set; }   // A unique identifier for the device
    }
}