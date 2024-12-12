using API.Data.Pagination;
using API.DTOs;
using API.Entities;
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
                Include(u => u.EducationLevel).
                Include(u => u.EmploymentType).
                Include(u => u.ApplicantPreviousCompanies).
                Include(u => u.EmploymentStatus).
                Include(r => r.ApplicantEducations).
                //Include(r => r.UserJobSubcategories).
                Include(r => r.PricingPlan).
                Include(r => r.City).
                ThenInclude(r => r.Country);
        }

        public async Task<PagedList<UserJobPost>> GetJobPostsAsync(AdsPaginationParameters adsParameters)
        {
            var userJobPosts = FindByCondition(u =>
                (u.AdEndDate >= DateTime.UtcNow) && u.JobPostStatusId == (int)Helpers.JobPostStatus.Active &&
                (adsParameters.UserId == null || adsParameters.UserId == u.SubmittingUserId) &&
                (adsParameters.cityIds == null || !adsParameters.cityIds.Any() || adsParameters.cityIds.Contains(u.CityId)) &&
                (adsParameters.jobCategoryIds == null  || !adsParameters.jobCategoryIds.Any() || adsParameters.jobCategoryIds.Contains(u.JobCategoryId)) &&
                (adsParameters.jobTypeIds == null || !adsParameters.jobTypeIds.Any() || adsParameters.jobTypeIds.Contains(u.JobTypeId)) &&
                (adsParameters.advertisementTypeId == null || u.AdvertisementTypeId == adsParameters.advertisementTypeId) &&
                (adsParameters.StartPrice == null || u.Price >= adsParameters.StartPrice) &&
                (adsParameters.EndPrice == null || u.Price <= adsParameters.EndPrice) &&
                (
                    (adsParameters.fromDate == null && adsParameters.toDate == null) ||
                    (adsParameters.fromDate.HasValue && !adsParameters.toDate.HasValue && u.AdStartDate.Date >= adsParameters.fromDate.Value.Date) ||
                    (adsParameters.fromDate.HasValue  && adsParameters.toDate.HasValue && u.AdStartDate.Date >= adsParameters.fromDate.Value.Date && u.AdStartDate.Date <= adsParameters.toDate.Value.Date) ||
                    (!adsParameters.fromDate.HasValue && adsParameters.toDate.HasValue && u.AdStartDate.Date <= adsParameters.toDate.Value.Date))
                );

            if (!string.IsNullOrEmpty(adsParameters.searchKeyword))
            {
                var keyword = adsParameters.searchKeyword.ToLower();
                userJobPosts = userJobPosts.Where(u =>
                    (!string.IsNullOrEmpty(u.Position) && u.Position.ToLower().Contains(keyword)) ||
                    (!string.IsNullOrEmpty(u.Biography) && u.Biography.ToLower().Contains(keyword)) ||
                    (!string.IsNullOrEmpty(u.ApplicantFirstName) && u.ApplicantFirstName.ToLower().Contains(keyword)) ||
                    (!string.IsNullOrEmpty(u.ApplicantLastName) && u.ApplicantLastName.ToLower().Contains(keyword)) ||
                    (!string.IsNullOrEmpty(u.ApplicantEmail) && u.ApplicantEmail.ToLower().Contains(keyword)) ||
                    (!string.IsNullOrEmpty(u.ApplicantPhoneNumber) && u.ApplicantPhoneNumber.ToLower().Contains(keyword)) || 
                    (!string.IsNullOrEmpty(u.AdTitle) && u.AdTitle.ToLower().Contains(keyword)) ||
                    (!string.IsNullOrEmpty(u.AdAdditionalDescription) && u.AdAdditionalDescription.ToLower().Contains(keyword))
                );
            }

            userJobPosts = userJobPosts
                .Include(u => u.PricingPlan)
                .Include(u => u.City)
                .Include(u => u.JobType)
                .Include(u => u.JobCategory)
                .Include(u => u.EducationLevel)
                .Include(u => u.EmploymentType)
                .Include(u => u.EmploymentStatus)
                .Include(u => u.ApplicantPreviousCompanies)
                .OrderBy(u => u.PricingPlan.Priority)
                .ThenByDescending(u => u.AdStartDate);
           // userJobPosts = _sortHelper.ApplySort(userJobPosts, adsParameters.OrderBy);
            return await PagedList<UserJobPost>.ToPagedListAsync(userJobPosts, adsParameters.PageNumber, adsParameters.PageSize);
        }

        public async Task<PagedList<UserJobPost>> GetUserJobPostsAsync(AdsPaginationParameters adsParameters)
        {
            var userJobPosts = FindByCondition(u => u.SubmittingUserId == adsParameters.UserId 
                                && u.JobPostStatusId == (int)adsParameters.adStatus && u.AdvertisementTypeId == adsParameters.advertisementTypeId);
            //userJobPosts = _sortHelper.ApplySort(userJobPosts, adsParameters.OrderBy);
            userJobPosts = userJobPosts
            .Include(u => u.PricingPlan)
            .Include(u => u.City)
            .Include(u => u.JobType)
            .Include(u => u.JobCategory)
            .Include(u => u.EducationLevel)
            .Include(u => u.EmploymentType)
            .Include(u => u.EmploymentStatus)
            .Include(u => u.ApplicantPreviousCompanies)
            .OrderByDescending(r => r.AdStartDate);
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
                var pricingPlan = DataContext.PricingPlans.FirstOrDefault(r => r.Name.Equals(newUserJobPost.PricingPlan.Name) && r.AdActiveDays == newUserJobPost.PricingPlan.AdActiveDays);
                if (pricingPlan == null)
                    return null;
                newUserJobPost.PricingPlan = pricingPlan;
                newUserJobPost.AdStartDate = DateTime.UtcNow;
                newUserJobPost.AdEndDate = DateTime.UtcNow.AddDays(pricingPlan.AdActiveDays);
                await DataContext.UserJobPosts.AddAsync(newUserJobPost);
                var user = DataContext.Users.First(r => r.Id == newUserJobPost.SubmittingUserId);
                user.Credits -= 1;
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
                //existingUserJobPost.JobPostStatusId = updatedUserJobPost.JobPostStatusId;
                existingUserJobPost.CityId = updatedUserJobPost.CityId;
                existingUserJobPost.CvFilePath = updatedUserJobPost.CvFilePath;
                existingUserJobPost.AdTitle = updatedUserJobPost.AdTitle;
                existingUserJobPost.AdAdditionalDescription = updatedUserJobPost.AdAdditionalDescription;

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

        public async Task<UserJobPost> UpdateAdUserBaseInfo(UserAdBaseInfoRequest updatedUserJobPost)
        {
            var existingUserJobPost = await FetchUserAdWithNecessaryDependencies(updatedUserJobPost.UserAdId);
            if (existingUserJobPost != null)
            {
                existingUserJobPost.ApplicantFirstName = updatedUserJobPost.FirstName;
                existingUserJobPost.ApplicantLastName = updatedUserJobPost.LastName;
                existingUserJobPost.ApplicantGender = updatedUserJobPost.Gender;
                existingUserJobPost.ApplicantDateOfBirth = updatedUserJobPost.DateOfBirth;
                existingUserJobPost.CityId = updatedUserJobPost.CityId;
                existingUserJobPost.ApplicantPhoneNumber = updatedUserJobPost.PhoneNumber;
                existingUserJobPost.ApplicantEmail = updatedUserJobPost.Email;
                await DataContext.SaveChangesAsync();
                await DataContext.Entry(existingUserJobPost)
                .Reference(ujp => ujp.City)
                .LoadAsync();
                return existingUserJobPost;
            }
            return null;
        }

        public async Task<UserJobPost> UpdateAdInfo(UserAdInfoUpdateRequest updateAdInfo)
        {
            var existingUserJobPost = await FetchUserAdWithNecessaryDependencies(updateAdInfo.UserAdId);
            if (existingUserJobPost != null)
            {
                existingUserJobPost.AdTitle = updateAdInfo.AdTitle;
                existingUserJobPost.JobCategoryId = updateAdInfo.JobCategoryId;
                existingUserJobPost.JobTypeId = updateAdInfo.JobTypeId;
                existingUserJobPost.Position = updateAdInfo.Position;
                existingUserJobPost.Biography = updateAdInfo.Biography;
                existingUserJobPost.YearsOfExperience = updateAdInfo.YearsOfExperience;
                existingUserJobPost.EmploymentStatusId = updateAdInfo.EmploymentStatusId;
                existingUserJobPost.EmploymentTypeId = updateAdInfo.EmploymentTypeId;
                existingUserJobPost.EducationLevelId = updateAdInfo.EducationLevelId;
                //existingUserJobPost.Price = updateAdInfo.Pr
                await DataContext.SaveChangesAsync();
                await DataContext.Entry(existingUserJobPost)
                .Reference(ujp => ujp.JobCategory)
                .LoadAsync();

                await DataContext.Entry(existingUserJobPost)
                    .Reference(ujp => ujp.JobType)
                    .LoadAsync();
                return existingUserJobPost;
            }
            return null;
        }

        public async Task<UserJobPost> UpsertApplicantEducationAsync(ApplicantEducationRequest userEducation)
        {
            if (userEducation.UserEducationId == null)
            {
                var newEducation = new ApplicantEducation()
                {
                    Degree = userEducation.Degree,
                    EducationEndYear = userEducation.EducationEndYear,
                    EducationStartYear = userEducation.EducationStartYear,
                    FieldOfStudy = userEducation.FieldOfStudy,
                    InstitutionName = userEducation.InstitutionName,
                    University = userEducation.University,
                    UserJobPostId = userEducation.UserAdId
                };
                await DataContext.ApplicantEducations.AddAsync(newEducation);
            }
            else
            {
                var existingEducation = DataContext.ApplicantEducations.First(r => r.Id == userEducation.UserEducationId && r.UserJobPostId == userEducation.UserAdId);
                if (existingEducation == null)
                    return null;
                existingEducation.EducationStartYear = userEducation.EducationStartYear;
                existingEducation.EducationEndYear = userEducation.EducationEndYear;
                existingEducation.University = userEducation.University;
                existingEducation.InstitutionName = userEducation.InstitutionName;
                existingEducation.FieldOfStudy = userEducation.FieldOfStudy;
                existingEducation.Degree = userEducation.Degree;
            }

            await DataContext.SaveChangesAsync();
            var existingUserJobPost = await FetchUserAdWithNecessaryDependencies(userEducation.UserAdId);
            return existingUserJobPost;
        }

        public async Task<UserJobPost> UpsertApplicantCompanyAsync(ApplicantCompanyRequst req)
        {
            if (req.UserCompanyId == null)
            {
                var item = new ApplicantPreviousCompanies()
                {
                    Position = req.Position,
                    CompanyName = req.CompanyName,
                    Description = req.Description,
                    StartYear = req.StartYear,
                    EndYear = req.EndYear,
                    UserJobPostId = req.UserAdId
                };
                await DataContext.ApplicantPreviousCompanies.AddAsync(item);
            }
            else
            {
                var existing = DataContext.ApplicantPreviousCompanies.First(r => r.Id == req.UserCompanyId && r.UserJobPostId == req.UserAdId);
                if (existing == null)
                    return null;
                existing.Position = req.Position;
                existing.CompanyName = req.CompanyName;
                existing.StartYear = req.StartYear;
                existing.EndYear = req.EndYear;
                existing.Description = req.Description;
            }

            await DataContext.SaveChangesAsync();
            var existingUserJobPost = await FetchUserAdWithNecessaryDependencies(req.UserAdId);
            return existingUserJobPost;
        }

        private async Task<UserJobPost> FetchUserAdWithNecessaryDependencies(int adId)
        {
            var existingUserJobPost = await DataContext.UserJobPosts
                .Include(ujp => ujp.JobCategory)
                .Include(ujp => ujp.JobType)
                .Include(ujp => ujp.City)
                .Include(r => r.ApplicantEducations)
                .Include(u => u.EducationLevel)
                .Include(u => u.EmploymentType)
                .Include(u => u.EmploymentStatus)
                .Include(u => u.ApplicantPreviousCompanies)
                .FirstOrDefaultAsync(ujp => ujp.Id == adId);
            return existingUserJobPost;
        }

        public async Task<UserJobPost> DeleteApplicantEducationByIdAsync(int educationId)
        {
            var existingEducation = DataContext.ApplicantEducations.First(r => r.Id == educationId);
            if (existingEducation == null)
                return null;
            var adId = existingEducation.UserJobPostId;
            DataContext.ApplicantEducations.Remove(existingEducation);
            await DataContext.SaveChangesAsync();
            var existingUserJobPost = await FetchUserAdWithNecessaryDependencies(adId);
            return existingUserJobPost;
        }

        public async Task<UserJobPost> DeleteApplicantCompanyByIdAsync(int id)
        {
            var existing = DataContext.ApplicantPreviousCompanies.First(r => r.Id == id);
            if (existing == null)
                return null;
            var adId = existing.UserJobPostId;
            DataContext.ApplicantPreviousCompanies.Remove(existing);
            await DataContext.SaveChangesAsync();
            var existingUserJobPost = await FetchUserAdWithNecessaryDependencies(adId);
            return existingUserJobPost;
        }

        public async Task<List<ApplicantEducation>> GetAllEducationsByAdIdAsync(int adId)
        {
            var educations = await DataContext.ApplicantEducations.Where(r => r.UserJobPostId == adId).ToListAsync();
            return educations;
        }

        public async Task<List<ApplicantPreviousCompanies>> GetAllApplicantCompaniesByAdIdAsync(int adId)
        {
            var items = await DataContext.ApplicantPreviousCompanies.Where(r => r.UserJobPostId == adId).ToListAsync();
            return items;
        }

        public async Task<UserJobPost> UpdateUserAdCvFilePathAsync(int userAdId, string cvFilePath, string cvFileName)
        {
            var userAd = await DataContext.UserJobPosts.FirstAsync(r => r.Id == userAdId);
            if(userAd != null)
            {
                userAd.CvFileName = cvFileName;
                userAd.CvFilePath = cvFilePath;
                await DataContext.SaveChangesAsync();
                var existingUserJobPost = await FetchUserAdWithNecessaryDependencies(userAdId);
                return existingUserJobPost;
            }
            return null;
        }
        public async Task<UserJobPost> DeleteUserAdFileAsync(int userAdId)
        {
            var userAd = await DataContext.UserJobPosts.FirstAsync(r => r.Id == userAdId);
            if (userAd != null)
            {
                userAd.CvFileName = null;
                userAd.CvFilePath = null;
                await DataContext.SaveChangesAsync();
                var existingUserJobPost = await FetchUserAdWithNecessaryDependencies(userAdId);
                return existingUserJobPost;
            }
            return null;
        }
    }
}
