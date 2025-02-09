using API.Data.Pagination;
using API.Entities;
using API.Entities.CompanyJobPost;
using API.Entities.JobPost;
using API.PaginationEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Data.ICompanyJobPostRepository
{
    public interface ICompanyJobPostRepository
    {
        Task<PagedList<CompanyJobPost>> GetJobPostsAsync(AdsPaginationParameters adsParameters);
        Task<PagedList<Company>> GetRegisteredCompaniesAsync(AdsPaginationParameters adsParameters);
        Task<PagedList<CompanyJobPost>> GetCompanyJobPostsAsync(AdsPaginationParameters adsParameters);
        Task<CompanyJobPost> GetCompanyJobPostByIdAsync(int id);
        Task<CompanyJobPost> CreateCompanyJobPostAsync(CompanyJobPost companyJobPost);
        Task<CompanyJobPost> UpdateCompanyJobPostAsync(CompanyJobPost companyJobPost);
        Task<CompanyJobPost> UpdateCompensationAndWorkEnvAsync(CompanyJobPost companyJobPost);
        Task<CompanyJobPost> UpdateQualificationsAndExperienceAsync(CompanyJobPost companyJobPost);
        Task<CompanyJobPost> UpdateHowToApplyAsync(CompanyJobPost companyJobPost);

        Task<List<CompanyJobPost>> GetCompanyAdsAsync(int companyId);
        Task<bool> DeleteCompanyJobPostByIdAsync(int id);
        Task<bool> CloseCompanyJobPostByIdAsync(int id);
        Task<bool> ReactivateCompanyJobPostByIdAsync(int id);
        Task<List<EmploymentType>> GetEmploymentTypesAsync();
        Task<List<EducationLevel>> GetEducationLevels();
        Task<List<EmploymentStatus>> GetEmploymentStatusesAsync();
        Task<List<CompanyJobPost>> GetCompanyActiveAdsAsync(int companyId);
        Task<bool> UpdateCompanyJobPostLogoAsync(int id, string logoUrl);
    }
}
