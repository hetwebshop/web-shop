using API.Data.Pagination;
using API.DTOs;
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
        Task<PagedList<UserJobPost>> GetUserJobPostsAsync(AdsPaginationParameters adsParameters);
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
        Task<UserJobPost> UpdateAdUserBaseInfo(UserAdBaseInfoRequest userAdBaseInfoRequest);
        Task<UserJobPost> UpdateAdInfo(UserAdInfoUpdateRequest userAdInfoUpdateRequest);
        Task<UserJobPost> UpsertApplicantEducationAsync(ApplicantEducationRequest req);
        Task<UserJobPost> DeleteApplicantEducationByIdAsync(int educationId);
        Task<List<ApplicantEducation>> GetAllEducationsByAdIdAsync(int adId);
        Task<UserJobPost> UpdateUserAdCvFilePathAsync(int userAdId, string cvFilePath, string cvFileName);
        Task<UserJobPost> DeleteUserAdFileAsync(int userAdId);
    }
}
