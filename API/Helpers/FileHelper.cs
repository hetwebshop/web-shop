using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;

namespace API.Helpers
{
    public static class FileHelper
    {
        public static bool IsValidPdf(IFormFile file)
        {
            if (file == null)
                return true;
            var allowedExtension = ".pdf";
            var allowedMimeType = "application/pdf";

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (extension != allowedExtension)
                return false;

            if (file.ContentType.ToLower() != allowedMimeType)
                return false;

            return true;
        }

        public static bool IsValidImage(IFormFile file)
        {
            var allowedExtensions = new HashSet<string> { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            var allowedMimeTypes = new HashSet<string> { "image/jpeg", "image/png", "image/gif", "image/bmp", "image/webp" };

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
                return false;

            if (!allowedMimeTypes.Contains(file.ContentType.ToLower()))
                return false;

            return true;
        }
    }
}
