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
        Task<List<UserEducation>> GetAllUserEducationsAsync(int userId);
        Task<bool> RemoveAllUserEducationsAsync(int userId);
        Task<User> GetUserByIdAsync(int userId);
        Task<bool> UpdateUserEducationAsync(UserEducationRequest userEducation);
        Task<bool> UpdateUserPreviousCompaniesAsync(UserCompanyRequest userEducation);
        Task<bool> UpdateUserPhotoUrl(User user, string photoUrl);
    }
}
