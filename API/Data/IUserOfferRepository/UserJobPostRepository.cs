using API.Data.Pagination;
using API.Entities.JobPost;
using API.Helpers;
using API.PaginationEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace API.Data.IUserOfferRepository
{
    public class UserJobPostRepository : RepositoryBase<UserJobPost>, IUserJobPostRepository
    {
        ISortHelper<UserJobPost> _sortHelper;

        public UserJobPostRepository(DataContext dataContext, ISortHelper<UserJobPost> sortHelper)
            : base(dataContext)
        {
            _sortHelper = sortHelper;
        }

        private IQueryable<UserJobPost> GetUserJobPostBaseQuery()
        {
            return DataContext.UserJobPosts.
                Include(r => r.JobCategory).
                Include(r => r.JobPostStatus).
                Include(r => r.JobType).
                Include(r => r.User).
                Include(r => r.ApplicantEducations).
                //Include(r => r.UserJobSubcategories).
                Include(r => r.City).
                ThenInclude(r => r.Country);
        }

        public async Task<PagedList<UserJobPost>> GetJobPostsAsync(AdsPaginationParameters adsParameters)
        {
            var userJobPosts = FindByCondition(u =>
                (u.AdEndDate >= DateTime.UtcNow) && 
                (adsParameters.UserId == null || adsParameters.UserId == u.SubmittingUserId) &&
                (adsParameters.cityIds == null || adsParameters.cityIds.Contains(u.CityId)) &&
                (adsParameters.jobCategoryIds == null || adsParameters.jobCategoryIds.Contains(u.JobCategoryId)) &&
                (adsParameters.jobTypeIds == null || adsParameters.jobTypeIds.Contains(u.JobTypeId)) &&
                (adsParameters.advertisementTypeId == null || u.AdvertisementTypeId == adsParameters.advertisementTypeId) &&
                (
                    (adsParameters.fromDate == null && adsParameters.toDate == null) ||
                    (adsParameters.fromDate.HasValue && !adsParameters.toDate.HasValue && u.CreatedAt.Date >= adsParameters.fromDate.Value.Date) ||
                    (adsParameters.fromDate.HasValue  && adsParameters.toDate.HasValue && u.CreatedAt.Date >= adsParameters.fromDate.Value.Date && u.CreatedAt.Date <= adsParameters.toDate.Value.Date) ||
                    (!adsParameters.fromDate.HasValue && adsParameters.toDate.HasValue && u.CreatedAt.Date <= adsParameters.toDate.Value.Date))
                );
            
            //Add method to search by keyword here

            userJobPosts = _sortHelper.ApplySort(userJobPosts, adsParameters.OrderBy);
            return await PagedList<UserJobPost>.ToPagedListAsync(userJobPosts, adsParameters.PageNumber, adsParameters.PageSize);
        }

        public async Task<List<UserJobPost>> GetMyAdsAsync(int userId)
        {
            var userJobPosts = await GetUserJobPostBaseQuery().Where(r => r.SubmittingUserId == userId).ToListAsync();
            return userJobPosts;
        }

        public async Task<UserJobPost> GetUserJobPostByIdAsync(int id)
        {
            var userJobPost = await GetUserJobPostBaseQuery().FirstOrDefaultAsync(r => r.Id == id);
            return userJobPost;
        }

        public async Task<UserJobPost> CreateUserJobPostAsync(UserJobPost newUserJobPost)
        {
            try
            {
                await DataContext.UserJobPosts.AddAsync(newUserJobPost);
                await DataContext.SaveChangesAsync();
                return newUserJobPost;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<UserJobPost> UpdateUserJobPostAsync(UserJobPost updatedUserJobPost)
        {
            var existingUserJobPost = await DataContext.UserJobPosts.FindAsync(updatedUserJobPost.Id);

            if (existingUserJobPost != null)
            {
                var applicantEducations = await DataContext.ApplicantEducations.Where(r => r.UserJobPostId == updatedUserJobPost.Id).ToListAsync();
                if (applicantEducations.Any())
                {
                    DataContext.ApplicantEducations.RemoveRange(applicantEducations);
                }
                existingUserJobPost.Position = updatedUserJobPost.Position;
                existingUserJobPost.Biography = updatedUserJobPost.Biography;
                //existingUserJobPost.Skills = updatedUserJobPost.Skills;
                existingUserJobPost.ApplicantFirstName = updatedUserJobPost.ApplicantFirstName;
                existingUserJobPost.ApplicantLastName = updatedUserJobPost.ApplicantLastName;
                existingUserJobPost.ApplicantEmail = updatedUserJobPost.ApplicantEmail;
                existingUserJobPost.ApplicantPhoneNumber = updatedUserJobPost.ApplicantPhoneNumber;
                existingUserJobPost.ApplicantDateOfBirth = updatedUserJobPost.ApplicantDateOfBirth;
                existingUserJobPost.ApplicantGender = updatedUserJobPost.ApplicantGender;
                existingUserJobPost.Price = updatedUserJobPost.Price;
                //existingUserJobPost.Tags = updatedUserJobPost.Tags;
                existingUserJobPost.CreatedAt = updatedUserJobPost.CreatedAt;
                existingUserJobPost.UpdatedAt = updatedUserJobPost.UpdatedAt;
                existingUserJobPost.JobTypeId = updatedUserJobPost.JobTypeId;
                existingUserJobPost.JobCategoryId = updatedUserJobPost.JobCategoryId;
                existingUserJobPost.JobPostStatusId = updatedUserJobPost.JobPostStatusId;
                existingUserJobPost.CityId = updatedUserJobPost.CityId;

                // Update related collections
                existingUserJobPost.ApplicantEducations = updatedUserJobPost.ApplicantEducations;
                //existingUserJobPost.UserJobSubcategories = updatedUserJobPost.UserJobSubcategories;

                await DataContext.SaveChangesAsync();
            }

            var updatedItem = await DataContext.UserJobPosts.FindAsync(updatedUserJobPost.Id);
            return updatedItem;
        }


        public async Task<List<JobCategory>> GetAllJobCategoriesAsync()
        {
            var jobCategories = await DataContext.JobCategories.ToListAsync();
            return jobCategories;
        }

        public async Task<List<JobType>> GetAllJobTypesAsync()
        {
            var jobTypes = await DataContext.JobTypes.ToListAsync();
            return jobTypes;
        }

        public async Task<List<AdvertisementType>> GetAllAdvertisementTypesAsync()
        {
            var adTypes = await DataContext.AdvertisementTypes.ToListAsync();
            return adTypes;
        }

        public async Task<bool> DeleteUserJobPostByIdAsync(int id)
        {
            var userJobPost = await DataContext.UserJobPosts.FindAsync(id);
            if(userJobPost != null)
            {
                userJobPost.IsDeleted = true;
                userJobPost.JobPostStatusId = (int)Helpers.JobPostStatus.Deleted;
                await DataContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> CloseUserJobPostByIdAsync(int id)
        {
            var userJobPost = await DataContext.UserJobPosts.FindAsync(id);
            if (userJobPost != null)
            {
                userJobPost.JobPostStatusId = (int)Helpers.JobPostStatus.Closed;
                await DataContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> ReactivateUserJobPostByIdAsync(int id)
        {
            var userJobPost = await DataContext.UserJobPosts.FindAsync(id);
            if (userJobPost != null)
            {
                userJobPost.JobPostStatusId = (int)Helpers.JobPostStatus.Active;
                await DataContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
