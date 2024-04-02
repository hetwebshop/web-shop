using System.Collections.Generic;
using API.DTOs;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using API.Entities;

namespace API.Data
{
    public interface IUserRepository
    {
        Task<int> GetUserIdByUserName(string userName);
        Task<string> GetUserNameById(int id);
        Task<bool> UserExist(int id);
        Task<UserInfoDto> GetUserInfo(string userName);
        Task<bool> UserExist(string userName);
        Task<UserProfileDto> GetProfile(int id);
        Task<string> UpdateUserPhoto(IFormFile file, int userId);
        Task DeleteUserPhoto(int userId);
        Task<List<UserEducation>> GetAllUserEducationsAsync(int userId);
        Task<bool> RemoveAllUserEducationsAsync(int userId);
    }
}
