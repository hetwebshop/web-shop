using API.Data.Pagination;
using API.Entities.JobPost;
using API.Helpers;
using API.PaginationEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Data.IUserOfferRepository
{
    public interface IUserJobPostRepository
    {
        Task<PagedList<UserJobPost>> GetJobPostsAsync(AdsPaginationParameters adsParameters);
        Task<UserJobPost> GetUserJobPostByIdAsync(int id);
        Task<UserJobPost> CreateUserJobPostAsync(UserJobPost userJobPost);
        Task<UserJobPost> UpdateUserJobPostAsync(UserJobPost userJobPost);
        Task<List<JobCategory>> GetAllJobCategoriesAsync();
        Task<List<JobType>> GetAllJobTypesAsync();
        Task<List<AdvertisementType>> GetAllAdvertisementTypesAsync();
        Task<List<UserJobPost>> GetMyAdsAsync(int userId);
        Task<bool> DeleteUserJobPostByIdAsync(int id);
        Task<bool> CloseUserJobPostByIdAsync(int id);
        Task<bool> ReactivateUserJobPostByIdAsync(int id);
    }
}
