using API.Entities.JobPost;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Data.IUserOfferRepository
{
    public interface IUserJobPostRepository
    {
        Task<List<UserJobPost>> GetAllUserJobPostsAsync();
        Task<UserJobPost> GetUserJobPostByIdAsync(int id);
        Task<UserJobPost> CreateUserJobPostAsync(UserJobPost userJobPost);
        Task<UserJobPost> UpdateUserJobPostAsync(UserJobPost userJobPost);
        Task<List<JobCategory>> GetAllJobCategoriesAsync();
        Task<List<JobType>> GetAllJobTypesAsync();
        Task<List<AdvertisementType>> GetAllAdvertisementTypesAsync();
        Task<List<UserJobPost>> GetMyAdsAsync(int userId);
    }
}
