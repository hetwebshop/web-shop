namespace API.DTOs
{
    public class GoogleLoginDto
    {
        public string IdToken { get; set; }
        public string IPAddress { get; set; }
        public string UserAgent { get; set; }
        public string DeviceId { get; set; }
    }
}
