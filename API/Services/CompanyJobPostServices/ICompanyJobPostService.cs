﻿using API.Data.Pagination;
using API.DTOs;
using API.PaginationEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services.CompanyJobPostServices
{
    public interface ICompanyJobPostService
    {
        Task<PagedList<CompanyJobPostDto>> GetJobPostsAsync(AdsPaginationParameters adsParameters);
        Task<PagedList<CompanyPublicInfoDto>> GetRegisteredCompaniesAsync(AdsPaginationParameters adsParameters);
        Task<PagedList<CompanyJobPostDto>> GetCompanyJobPostsAsync(AdsPaginationParameters adsParameters);
        Task<CompanyJobPostDto> GetCompanyJobPostByIdAsync(int id);
        Task<CompanyJobPostDto> CreateCompanyJobPostAsync(CompanyJobPostDto userJobPost);
        Task<CompanyJobPostDto> UpdateCompanyJobPostAsync(CompanyJobPostDto userJobPost);
        Task<CompanyJobPostDto> UpdateCompensationAndWorkEnvAsync(CompanyJobPostDto userJobPost);
        Task<CompanyJobPostDto> UpdateQualificationsAndExpereinceAsync(CompanyJobPostDto userJobPost);
        Task<CompanyJobPostDto> UpdateHowToApplyAsync(CompanyJobPostDto userJobPost);

        Task<List<CompanyJobPostDto>> GetCompanyAdsAsync(int companyId);
        Task<bool> DeleteCompanyJobPostByIdAsync(int companyId, int jobPostId);
        Task<bool> CloseCompanyJobPostByIdAsync(int companyId, int jobPostId);
        Task<bool> ReactivateCompanyJobPostByIdAsync(int companyId, int jobPostId);
        Task<bool> UpdateCompanyJobPostLogoAsync(int id, string logoUrl);
    }
}
