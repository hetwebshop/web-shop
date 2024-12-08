using API.Data.Pagination;
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
        Task<PagedList<CompanyJobPost>> GetCompanyJobPostsAsync(AdsPaginationParameters adsParameters);
        Task<CompanyJobPost> GetCompanyJobPostByIdAsync(int id);
        Task<CompanyJobPost> CreateCompanyJobPostAsync(CompanyJobPost companyJobPost);
        Task<CompanyJobPost> UpdateCompanyJobPostAsync(CompanyJobPost companyJobPost);
        Task<List<CompanyJobPost>> GetCompanyAdsAsync(int companyId);
        Task<bool> DeleteCompanyJobPostByIdAsync(int id);
        Task<bool> CloseCompanyJobPostByIdAsync(int id);
        Task<bool> ReactivateCompanyJobPostByIdAsync(int id);
        Task<List<EmploymentType>> GetEmploymentTypesAsync();
    }
}
