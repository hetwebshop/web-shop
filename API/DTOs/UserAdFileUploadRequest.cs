using Microsoft.AspNetCore.Http;

namespace API.DTOs
{
    public class UserAdFileUploadRequest
    {
        public IFormFile CvFile { get; set; }
    }
}
