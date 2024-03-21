using API.Entities.JobPost;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data.IUserOfferRepository
{
    public class UserJobPostRepository : IUserJobPostRepository
    {
        public DataContext DataContext { get; }

        public UserJobPostRepository(DataContext dataContext)
        {
            DataContext = dataContext;
        }

        private IQueryable<UserJobPost> GetUserJobPostBaseQuery()
        {
            return DataContext.UserJobPosts.
                Include(r => r.JobCategory).
                Include(r => r.JobPostStatus).
                Include(r => r.JobType).
                Include(r => r.User).
                Include(r => r.ApplicantEducations).
                Include(r => r.UserJobSubcategories).
                Include(r => r.City).
                ThenInclude(r => r.Country);
        }

        public async Task<List<UserJobPost>> GetAllUserJobPostsAsync()
        {
            var userJobPosts = await GetUserJobPostBaseQuery().ToListAsync();
            return userJobPosts;
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
                var userJobSubcategories = await DataContext.UserJobSubcategories.Where(r => r.UserJobPostId == updatedUserJobPost.Id).ToListAsync();
                if (userJobSubcategories.Any())
                {
                    DataContext.UserJobSubcategories.RemoveRange(userJobSubcategories);
                }
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
                existingUserJobPost.SubmittingUserId = updatedUserJobPost.SubmittingUserId;
                existingUserJobPost.JobTypeId = updatedUserJobPost.JobTypeId;
                existingUserJobPost.JobCategoryId = updatedUserJobPost.JobCategoryId;
                existingUserJobPost.JobPostStatusId = updatedUserJobPost.JobPostStatusId;
                existingUserJobPost.CityId = updatedUserJobPost.CityId;

                // Update related collections
                existingUserJobPost.ApplicantEducations = updatedUserJobPost.ApplicantEducations;
                existingUserJobPost.UserJobSubcategories = updatedUserJobPost.UserJobSubcategories;

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
    }
}
