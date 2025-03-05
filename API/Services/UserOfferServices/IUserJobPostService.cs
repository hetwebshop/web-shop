using API.Data.Pagination;
using API.DTOs;
using API.Entities;
using API.Entities.JobPost;
using API.Helpers;
using API.PaginationEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services.UserOfferServices
{
    public interface IUserJobPostService
    {
        Task<PagedList<UserJobPostDto>> GetJobPostsAsync(AdsPaginationParameters adsParameters);
        Task<PagedList<UserJobPostDto>> GetUserJobPostsAsync(AdsPaginationParameters adsParameters);
        Task<UserJobPostDto> GetUserJobPostByIdAsync(int id);
        Task<UserJobPostDto> CreateUserJobPostAsync(UserJobPostDto userJobPost, User user);
        Task<UserApplicationDto> CreateUserApplicationAsync(UserApplicationDto userApplication, User user);
        Task<UserJobPostDto> UpdateUserJobPostAsync(UserJobPostDto userJobPost);
        Task<List<JobCategory>> GetAllJobCategoriesAsync();
        Task<List<JobType>> GetAllJobTypesAsync();
        Task<List<AdvertisementType>> GetAllAdvertisementTypesAsync();
        Task<List<UserJobPostDto>> GetMyAdsAsync(int userId);
        Task<bool> DeleteUserJobPostByIdAsync(int userId, int jobPostId);
        Task<bool> CloseUserJobPostByIdAsync(int userId, int jobPostId);
        Task<bool> ReactivateUserJobPostByIdAsync(int userId, int jobPostId);
        Task<UserJobPostDto> UpdateAdUserBaseInfo(UserAdBaseInfoRequest userAdBaseInfoRequest);
        Task<UserJobPostDto> UpdateAdInfo(UserAdInfoUpdateRequest userAdInfoUpdateRequest);
    }
}
