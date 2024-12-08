using API.Data.ICompanyJobPostRepository;
using API.Data.Pagination;
using API.DTOs;
using API.Mappers;
using API.PaginationEntities;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services.CompanyJobPostServices
{
    public class CompanyJobPostService : ICompanyJobPostService
    {
        private readonly ICompanyJobPostRepository companyJobPostRepository;

        public CompanyJobPostService(ICompanyJobPostRepository companyJobPostRepository)
        {
            this.companyJobPostRepository = companyJobPostRepository;
        }

        public async Task<PagedList<CompanyJobPostDto>> GetJobPostsAsync(AdsPaginationParameters adsParameters)
        {
            var companyJobPosts = await companyJobPostRepository.GetJobPostsAsync(adsParameters);
            return companyJobPosts.ToDto();
        }

        public async Task<PagedList<CompanyJobPostDto>> GetCompanyJobPostsAsync(AdsPaginationParameters adsParameters)
        {
            var companyJobPosts = await companyJobPostRepository.GetCompanyJobPostsAsync(adsParameters);
            return companyJobPosts.ToDto();
        }

        public async Task<CompanyJobPostDto> GetCompanyJobPostByIdAsync(int id)
        {
            var companyJobPost = await companyJobPostRepository.GetCompanyJobPostByIdAsync(id);
            return companyJobPost.ToDto();
        }

        public async Task<CompanyJobPostDto> CreateCompanyJobPostAsync(CompanyJobPostDto companyJobPostDto)
        {
            var newItem = await companyJobPostRepository.CreateCompanyJobPostAsync(companyJobPostDto.ToEntity());
            return newItem.ToDto();
        }

        public async Task<CompanyJobPostDto> UpdateCompanyJobPostAsync(CompanyJobPostDto companyJobPostDto)
        {
            try
            {
                var newItem = await companyJobPostRepository.UpdateCompanyJobPostAsync(companyJobPostDto.ToEntity());
                return newItem.ToDto();
            }
            catch (AutoMapperMappingException ex)
            {
                throw;
            }

        }

        public async Task<List<CompanyJobPostDto>> GetCompanyAdsAsync(int companyId)
        {
            var companyAds = await companyJobPostRepository.GetCompanyAdsAsync(companyId);
            return companyAds.ToDto();
        }

        public async Task<bool> DeleteCompanyJobPostByIdAsync(int companyId, int jobPostId)
        {
            var jobPost = await companyJobPostRepository.GetCompanyJobPostByIdAsync(jobPostId);
            if (jobPost.User.CompanyId != companyId)
                return false;
            var deleted = await companyJobPostRepository.DeleteCompanyJobPostByIdAsync(jobPostId);
            return deleted;
        }

        public async Task<bool> CloseCompanyJobPostByIdAsync(int companyId, int jobPostId)
        {
            var jobPost = await companyJobPostRepository.GetCompanyJobPostByIdAsync(jobPostId);
            if (jobPost.User.CompanyId != companyId)
                return false;
            var closed = await companyJobPostRepository.CloseCompanyJobPostByIdAsync(jobPostId);
            return closed;
        }

        public async Task<bool> ReactivateCompanyJobPostByIdAsync(int companyId, int jobPostId)
        {
            var jobPost = await companyJobPostRepository.GetCompanyJobPostByIdAsync(jobPostId);
            if (jobPost.User.CompanyId != companyId)
                return false;
            var reactivated = await companyJobPostRepository.ReactivateCompanyJobPostByIdAsync(jobPostId);
            return reactivated;
        }
    }
}
