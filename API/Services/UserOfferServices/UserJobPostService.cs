using API.Data;
using API.Data.IUserOfferRepository;
using API.Data.Pagination;
using API.DTOs;
using API.Entities.JobPost;
using API.Helpers;
using API.Mappers;
using API.PaginationEntities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services.UserOfferServices
{
    public class UserJobPostService : IUserJobPostService
    {
        private readonly IUserJobPostRepository userJobPostRepository;
        private readonly IUnitOfWork _uow;

        public UserJobPostService(IUserJobPostRepository userJobPostRepository, IUnitOfWork uow)
        {
            this.userJobPostRepository = userJobPostRepository;
            _uow = uow;
        }

        public async Task<PagedList<UserJobPostDto>> GetJobPostsAsync(AdsPaginationParameters adsParameters)
        {
            var userJobPosts = await userJobPostRepository.GetJobPostsAsync(adsParameters);
            return userJobPosts.ToDto();
        }

        public async Task<PagedList<UserJobPostDto>> GetUserJobPostsAsync(AdsPaginationParameters adsParameters)
        {
            var userJobPosts = await userJobPostRepository.GetUserJobPostsAsync(adsParameters);
            return userJobPosts.ToDto();
        }

        public async Task<UserJobPostDto> GetUserJobPostByIdAsync(int id)
        {
            var userJobPost = await userJobPostRepository.GetUserJobPostByIdAsync(id);
            return userJobPost.ToDto();
        }

        public async Task<UserJobPostDto> CreateUserJobPostAsync(UserJobPostDto userJobPostDto)
        {
            try
            {
                var entity = userJobPostDto.ToEntity();
                var newItem = await userJobPostRepository.CreateUserJobPostAsync(entity);
                if (newItem == null)
                    throw new Exception("Pretplata koju ste postavili ne postoji");
                var dto = newItem.ToDto();
                var user = await _uow.UserRepository.GetUserByIdAsync(dto.SubmittingUserId);
                dto.CurrentUserCredits = user.Credits;
                return dto;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<UserJobPostDto> UpdateUserJobPostAsync(UserJobPostDto userJobPostDto)
        {
            var newItem = await userJobPostRepository.UpdateUserJobPostAsync(userJobPostDto.ToEntity());
            return newItem.ToDto();
        }

        public async Task<List<JobCategory>> GetAllJobCategoriesAsync()
        {
            var jobCategories = await userJobPostRepository.GetAllJobCategoriesAsync();
            return jobCategories;
        }

        public async Task<List<JobType>> GetAllJobTypesAsync()
        {
            var jobTypes = await userJobPostRepository.GetAllJobTypesAsync();
            return jobTypes;
        }

        public async Task<List<AdvertisementType>> GetAllAdvertisementTypesAsync()
        {
            var adTypes = await userJobPostRepository.GetAllAdvertisementTypesAsync();
            return adTypes;
        }

        public async Task<List<UserJobPostDto>> GetMyAdsAsync(int userId)
        {
            var myAds = await userJobPostRepository.GetMyAdsAsync(userId);
            return myAds.ToDto();
        }

        public async Task<bool> DeleteUserJobPostByIdAsync(int userId, int jobPostId)
        {
            var jobPost = await userJobPostRepository.GetUserJobPostByIdAsync(jobPostId);
            if (jobPost.SubmittingUserId != userId)
                return false;
            var deleted = await userJobPostRepository.DeleteUserJobPostByIdAsync(jobPostId);
            return deleted;
        }

        public async Task<bool> CloseUserJobPostByIdAsync(int userId, int jobPostId)
        {
            var jobPost = await userJobPostRepository.GetUserJobPostByIdAsync(jobPostId);
            if (jobPost.SubmittingUserId != userId)
                return false;
            var closed = await userJobPostRepository.CloseUserJobPostByIdAsync(jobPostId);
            return closed;
        }

        public async Task<bool> ReactivateUserJobPostByIdAsync(int userId, int jobPostId)
        {
            var jobPost = await userJobPostRepository.GetUserJobPostByIdAsync(jobPostId);
            if (jobPost.SubmittingUserId != userId)
                return false;
            var reactivated = await userJobPostRepository.ReactivateUserJobPostByIdAsync(jobPostId);
            return reactivated;
        }

        public async Task<UserJobPostDto> UpdateAdUserBaseInfo(UserAdBaseInfoRequest userAdBaseInfoRequest)
        {
            var updated = await userJobPostRepository.UpdateAdUserBaseInfo(userAdBaseInfoRequest);
            return updated.ToDto();
        }
        public async Task<UserJobPostDto> UpdateAdInfo(UserAdInfoUpdateRequest userAdInfoUpdateRequest)
        {
            var updated = await userJobPostRepository.UpdateAdInfo(userAdInfoUpdateRequest);
            var dto = updated.ToDto();
            return dto;
        }
    }
}
