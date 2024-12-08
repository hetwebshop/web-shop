using Microsoft.AspNetCore.Http;

namespace API.DTOs
{
    public class UserUploadResumeRequest
    {
        public IFormFile CvFile { get; set; }
    }
}
