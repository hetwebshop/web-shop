using API.Data.Pagination;
using API.Entities;
using API.Entities.CompanyJobPost;
using API.Entities.JobPost;
using API.Entities.Payment;
using API.PaginationEntities;
using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data.ICompanyJobPostRepository
{
    public class CompanyJobPostRepository : RepositoryBase<CompanyJobPost>, ICompanyJobPostRepository
    {
        ISortHelper<CompanyJobPost> _sortHelper;
        private readonly IBlobStorageService _blobStorageService;
        private readonly ILogger<CompanyJobPostRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly int CompanyActiveAdDays;

        public CompanyJobPostRepository(DataContext dataContext, ISortHelper<CompanyJobPost> sortHelper, IBlobStorageService blobStorageService, ILogger<CompanyJobPostRepository> logger, IConfiguration configuration)
            : base(dataContext)
        {
            _sortHelper = sortHelper;
            _blobStorageService = blobStorageService;
            _logger = logger;
            _configuration = configuration;
            CompanyActiveAdDays = int.Parse(_configuration.GetSection("CompanyActiveAdDays").Value);
        }

        private IQueryable<CompanyJobPost> GetCompanyJobPostBaseQuery()
        {
            return DataContext.CompanyJobPosts.
                Include(r => r.JobCategory).
                Include(r => r.JobPostStatus).
                Include(r => r.User).
                ThenInclude(r => r.Company).
                Include(r => r.PricingPlan).
                Include(r => r.EmploymentType).
                Include(r => r.UserApplications).
                Include(r => r.EducationLevel).
                Include(r => r.City).
                ThenInclude(r => r.Country).
                Include(r => r.JobType);
        }

        public async Task<PagedList<CompanyJobPost>> GetJobPostsAsync(AdsPaginationParameters adsParameters)
        {
            var companyJobPosts = FindByCondition(u =>
                (u.AdEndDate >= DateTime.UtcNow) &&
                (adsParameters.CompanyId == null || adsParameters.CompanyId == u.User.CompanyId) &&
                (adsParameters.adStatus == null || (int)adsParameters.adStatus == u.JobPostStatusId) &&
                (adsParameters.cityIds == null || !adsParameters.cityIds.Any() || adsParameters.cityIds.Contains(u.CityId)) &&
                (adsParameters.jobCategoryIds == null || !adsParameters.jobCategoryIds.Any() || adsParameters.jobCategoryIds.Contains(u.JobCategoryId)) &&
                (adsParameters.jobTypeIds == null || !adsParameters.jobTypeIds.Any() || adsParameters.jobTypeIds.Contains(u.JobTypeId)) &&
                (adsParameters.educationLevelIds == null || !adsParameters.educationLevelIds.Any() || adsParameters.educationLevelIds.Contains(u.EducationLevelId)) &&
                (adsParameters.employmentTypeIds == null || !adsParameters.employmentTypeIds.Any() || adsParameters.employmentTypeIds.Contains(u.EmploymentTypeId)) &&
                (adsParameters.minYearsOfExperience == null || adsParameters.minYearsOfExperience >= u.RequiredExperience || u.RequiredExperience == null) &&
                u.IsDeleted == false &&
                (
                    (adsParameters.fromDate == null && adsParameters.toDate == null) ||
                    (adsParameters.fromDate.HasValue && !adsParameters.toDate.HasValue && u.AdStartDate.Date >= adsParameters.fromDate.Value.Date) ||
                    (adsParameters.fromDate.HasValue && adsParameters.toDate.HasValue && u.AdStartDate.Date >= adsParameters.fromDate.Value.Date && u.AdEndDate.Date <= adsParameters.toDate.Value.Date) ||
                    (!adsParameters.fromDate.HasValue && adsParameters.toDate.HasValue && u.AdEndDate.Date <= adsParameters.toDate.Value.Date))
                );

            if (!string.IsNullOrEmpty(adsParameters.searchKeyword))
            {
                var keyword = adsParameters.searchKeyword.ToLower();
                companyJobPosts = companyJobPosts.Where(u =>
                    (!string.IsNullOrEmpty(u.Position) && u.Position.ToLower().Contains(keyword)) ||
                    (!string.IsNullOrEmpty(u.JobDescription) && u.JobDescription.ToLower().Contains(keyword)) ||
                    (!string.IsNullOrEmpty(u.AdName) && u.AdName.ToLower().Contains(keyword))
                );
            }

            companyJobPosts = companyJobPosts
                .Include(r => r.JobCategory).
                Include(r => r.JobPostStatus).
                Include(r => r.JobType).
                Include(r => r.User).
                ThenInclude(r => r.Company).
                Include(r => r.PricingPlan).
                Include(r => r.EducationLevel).
                Include(r => r.UserApplications).
                Include(r => r.City).
                ThenInclude(r => r.Country)
                    .OrderBy(u => u.PricingPlan.Priority)
                    .ThenByDescending(u => u.RefreshDateTime);
            //companyJobPosts = _sortHelper.ApplySort(companyJobPosts, adsParameters.OrderBy);
            return await PagedList<CompanyJobPost>.ToPagedListAsync(companyJobPosts, adsParameters.PageNumber, adsParameters.PageSize);
        }

        public async Task<PagedList<Company>> GetRegisteredCompaniesAsync(AdsPaginationParameters adsParameters)
        {
            var registeredCompanies = DataContext.Companies.Include(r => r.City);

            return await PagedList<Company>.ToPagedListAsync(registeredCompanies, adsParameters.PageNumber, adsParameters.PageSize);
        }

        public async Task<PagedList<CompanyJobPost>> GetCompanyJobPostsAsync(AdsPaginationParameters adsParameters)
        {
            var companyJobPosts = FindByCondition(u => u.SubmittingUserId == adsParameters.UserId
                                && u.JobPostStatusId == (int)adsParameters.adStatus);
            //companyJobPosts = _sortHelper.ApplySort(companyJobPosts, adsParameters.OrderBy);
            companyJobPosts = companyJobPosts
                .Include(r => r.JobCategory).
                Include(r => r.JobPostStatus).
                Include(r => r.JobType).
                Include(r => r.User).
                ThenInclude(r => r.Company).
                Include(r => r.PricingPlan).
                Include(r => r.EducationLevel).
                Include(r => r.UserApplications).
                Include(r => r.City).
                ThenInclude(r => r.Country)
                    .OrderByDescending(u => u.AdStartDate);
            return await PagedList<CompanyJobPost>.ToPagedListAsync(companyJobPosts, adsParameters.PageNumber, adsParameters.PageSize);
        }


        public async Task<List<CompanyJobPost>> GetCompanyAdsAsync(int companyId)
        {
            var userJobPosts = await GetCompanyJobPostBaseQuery().Where(r => r.User.CompanyId == companyId).ToListAsync();
            return userJobPosts;
        }
        
        public async Task<List<CompanyJobPost>> GetCompanyActiveAdsAsync(int companyId)
        {
            var userJobPosts = await GetCompanyJobPostBaseQuery().Where(r => r.User.CompanyId == companyId && r.AdEndDate.AddDays(CompanyActiveAdDays) >= DateTime.UtcNow).OrderByDescending(r => r.CreatedAt).ToListAsync();
            return userJobPosts;
        }

        public async Task<CompanyJobPost> GetCompanyJobPostByIdAsync(int id)
        {
            var userJobPost = await GetCompanyJobPostBaseQuery().FirstOrDefaultAsync(r => r.Id == id);
            return userJobPost;
        }

        public async Task<CompanyJobPost> CreateCompanyJobPostAsync(CompanyJobPost newCompanyJobPost)
        {
            //use transaction with isolation to ensure there is no race condition(2 users create job from 2 laptops with one shared account)
            using (var transaction = await DataContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable))
            {
                try
                {
                    var pricingPlan = await DataContext.PricingPlanCompanies.FirstOrDefaultAsync(r => r.Id == newCompanyJobPost.PricingPlanId);
                    if (pricingPlan == null)
                        throw new Exception("Odabran neispravan paket oglasa.");
                    var user = await DataContext.Users.FirstAsync(r => r.Id == newCompanyJobPost.SubmittingUserId);
                    if (user.Credits < pricingPlan.PriceInCredits)
                        throw new Exception("Korisnik nema dovoljno kredita za kreiranje odabranog oglasa. Molimo vas da dopunite kredite ili odaberete drugi paket oglasa.");

                    var now = DateTime.UtcNow;
                    if (pricingPlan.Name == "Premium")
                    {
                        //newCompanyJobPost.IsAiAnalysisIncluded = true;
                        newCompanyJobPost.AiAnalysisStatus = AiAnalysisStatus.Completed;
                        //newCompanyJobPost.AiAnalysisProgress = 100.00;
                    }
                    newCompanyJobPost.PricingPlanId = pricingPlan.Id;
                    newCompanyJobPost.AdStartDate = now;
                    newCompanyJobPost.AdEndDate = now.AddDays(pricingPlan.AdActiveDays);
                    newCompanyJobPost.RefreshDateTime = now;
                    newCompanyJobPost.RefreshIntervalInDays = pricingPlan.Name == "Premium" ? 3 : pricingPlan.Name == "Plus" ? 7 : null;

                    await DataContext.CompanyJobPosts.AddAsync(newCompanyJobPost);

                    
                    user.Credits -= pricingPlan.PriceInCredits;


                    var userTransaction = new UserTransaction()
                    {
                        Amount = pricingPlan.PriceInCredits,
                        UserId = user.Id,
                        CreatedAt = DateTime.UtcNow,
                        ChFullName = user.FirstName + " " + user.LastName,
                        IsProcessed = false,
                        TransactionType = TransactionType.PostingAd,
                        IsAddingCredits = false,
                        OrderInfo = OrderInfoMessages.PostingAdMessage
                    };

                    await DataContext.UserTransactions.AddAsync(userTransaction);

                    await DataContext.SaveChangesAsync(); 
                    await transaction.CommitAsync();

                    return newCompanyJobPost;
                }
                catch(Exception ex)
                {
                    _logger.LogError($"Database update failed for JobPost: {newCompanyJobPost.Id}, Error: {ex.Message}");
                    await transaction.RollbackAsync(); 
                    throw;
                }
            }
        }

        public async Task<CompanyJobPost> UpdateCompanyJobPostAsync(CompanyJobPost updatedCompanyJobPost)
        {
            try
            {
                var existingCompanyJobPost = await DataContext.CompanyJobPosts.FindAsync(updatedCompanyJobPost.Id);

                if (existingCompanyJobPost != null)
                {
                    existingCompanyJobPost.JobDescription = updatedCompanyJobPost.JobDescription;
                    //existingCompanyJobPost.CreatedAt = updatedCompanyJobPost.CreatedAt;
                    existingCompanyJobPost.UpdatedAt = updatedCompanyJobPost.UpdatedAt;
                    existingCompanyJobPost.JobTypeId = updatedCompanyJobPost.JobTypeId;
                    existingCompanyJobPost.JobCategoryId = updatedCompanyJobPost.JobCategoryId;
                    //existingCompanyJobPost.JobPostStatusId = updatedCompanyJobPost.JobPostStatusId;
                    existingCompanyJobPost.CityId = updatedCompanyJobPost.CityId;
                    existingCompanyJobPost.Position = updatedCompanyJobPost.Position;
                    existingCompanyJobPost.AdName = updatedCompanyJobPost.AdName;
                    existingCompanyJobPost.EmailForReceivingApplications = updatedCompanyJobPost.EmailForReceivingApplications;
                    existingCompanyJobPost.CompanyName = updatedCompanyJobPost.CompanyName;

                    await DataContext.SaveChangesAsync();
                }

                var updatedItem = await GetCompanyJobPostBaseQuery().Where(r => r.Id == updatedCompanyJobPost.Id).FirstAsync();

                return updatedItem;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<CompanyJobPost> UpdateCompensationAndWorkEnvAsync(CompanyJobPost updatedCompanyJobPost)
        {
            var existingCompanyJobPost = await DataContext.CompanyJobPosts.FindAsync(updatedCompanyJobPost.Id);

            if (existingCompanyJobPost != null)
            {
                existingCompanyJobPost.WorkEnvironmentDescription = updatedCompanyJobPost.WorkEnvironmentDescription;
                existingCompanyJobPost.UpdatedAt = updatedCompanyJobPost.UpdatedAt;
                existingCompanyJobPost.MaxSalary = updatedCompanyJobPost.MaxSalary;
                existingCompanyJobPost.MinSalary = updatedCompanyJobPost.MinSalary;
                existingCompanyJobPost.Benefits = updatedCompanyJobPost.Benefits;

                await DataContext.SaveChangesAsync();
            }

            var updatedItem = await GetCompanyJobPostBaseQuery().Where(r => r.Id == updatedCompanyJobPost.Id).FirstAsync();

            return updatedItem;
        }

        public async Task<bool> UpdateCompanyJobPostLogoAsync(int id, string logoUrl)
        {
            var existingCompanyJobPost = await DataContext.CompanyJobPosts.FindAsync(id);

            if (existingCompanyJobPost != null)
            {
                existingCompanyJobPost.PhotoUrl = logoUrl;

                await DataContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<CompanyJobPost> UpdateQualificationsAndExperienceAsync(CompanyJobPost updatedCompanyJobPost)
        {
            var existingCompanyJobPost = await DataContext.CompanyJobPosts.FindAsync(updatedCompanyJobPost.Id);

            if (existingCompanyJobPost != null)
            {
                existingCompanyJobPost.RequiredSkills = updatedCompanyJobPost.RequiredSkills;
                existingCompanyJobPost.UpdatedAt = updatedCompanyJobPost.UpdatedAt;
                existingCompanyJobPost.RequiredExperience = updatedCompanyJobPost.RequiredExperience;
                existingCompanyJobPost.EducationLevelId = updatedCompanyJobPost.EducationLevelId;
                existingCompanyJobPost.Certifications = updatedCompanyJobPost.Certifications;

                await DataContext.SaveChangesAsync();
            }

            var updatedItem = await GetCompanyJobPostBaseQuery().Where(r => r.Id == updatedCompanyJobPost.Id).FirstAsync();

            return updatedItem;
        }

        public async Task<CompanyJobPost> UpdateHowToApplyAsync(CompanyJobPost updatedCompanyJobPost)
        {
            var existingCompanyJobPost = await DataContext.CompanyJobPosts.FindAsync(updatedCompanyJobPost.Id);

            if (existingCompanyJobPost != null)
            {
                existingCompanyJobPost.HowToApply = updatedCompanyJobPost.HowToApply;
                existingCompanyJobPost.UpdatedAt = updatedCompanyJobPost.UpdatedAt;
                existingCompanyJobPost.DocumentsRequired = updatedCompanyJobPost.DocumentsRequired;

                await DataContext.SaveChangesAsync();
            }

            var updatedItem = await GetCompanyJobPostBaseQuery().Where(r => r.Id == updatedCompanyJobPost.Id).FirstAsync();

            return updatedItem;
        }

        public async Task<bool> DeleteCompanyJobPostByIdAsync(int id)
        {
            var companyJobPost = await DataContext.CompanyJobPosts.FindAsync(id);
            if (companyJobPost != null)
            {
                companyJobPost.IsDeleted = true;
                companyJobPost.JobPostStatusId = (int)Helpers.JobPostStatus.Deleted;
                await DataContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> CloseCompanyJobPostByIdAsync(int id)
        {
            var companyJobPost = await DataContext.CompanyJobPosts.FindAsync(id);
            if (companyJobPost != null)
            {
                companyJobPost.JobPostStatusId = (int)Helpers.JobPostStatus.Closed;
                companyJobPost.IsDeleted = false;
                await DataContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> ReactivateCompanyJobPostByIdAsync(int id)
        {
            var companyJobPost = await DataContext.CompanyJobPosts.FindAsync(id);
            if (companyJobPost != null)
            {
                companyJobPost.JobPostStatusId = (int)Helpers.JobPostStatus.Active;
                companyJobPost.IsDeleted = false;
                await DataContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<EmploymentType>> GetEmploymentTypesAsync()
        {
            var employmentTypes = await DataContext.EmploymentTypes.ToListAsync();
            return employmentTypes;
        }

        public async Task<List<EducationLevel>> GetEducationLevels()
        {
            var educationLevels = await DataContext.EducationLevels.ToListAsync();
            return educationLevels;
        }

        public async Task<List<EmploymentStatus>> GetEmploymentStatusesAsync()
        {
            var empStatuses = await DataContext.EmploymentStatuses.ToListAsync();
            return empStatuses;
        }
    }
}
