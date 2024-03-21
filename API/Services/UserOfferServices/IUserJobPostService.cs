using API.DTOs;
using API.Entities.JobPost;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services.UserOfferServices
{
    public interface IUserJobPostService
    {
        Task<List<UserJobPostDto>> GetAllUserJobPostsAsync();
        Task<UserJobPostDto> GetUserJobPostByIdAsync(int id);
        Task<UserJobPostDto> CreateUserJobPostAsync(UserJobPostDto userJobPost);
        Task<UserJobPostDto> UpdateUserJobPostAsync(UserJobPostDto userJobPost);
        Task<List<JobCategory>> GetAllJobCategoriesAsync();
        Task<List<JobType>> GetAllJobTypesAsync();
        Task<List<AdvertisementType>> GetAllAdvertisementTypesAsync();
        Task<List<UserJobPostDto>> GetMyAdsAsync(int userId);
    }
}
