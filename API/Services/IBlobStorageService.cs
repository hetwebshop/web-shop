using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IBlobStorageService
    {
        Task<string> UploadFileAsync(IFormFile file, int userId);
        Task<FileDto> GetFileAsync(string fileName);
        Task<bool> RemoveFileAsync(string fileName);
    }
}
