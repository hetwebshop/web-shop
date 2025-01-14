using API.Data.Pagination;
using API.DTOs;
using API.Entities;
using API.PaginationEntities;
using AutoMapper;
using System.Collections.Generic;

namespace API.Mappers
{
    public static class CompanyMapper
    {
        internal static IMapper Mapper { get; }

        static CompanyMapper()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<CompanyMapperProfile>()).CreateMapper();
        }

        public static CompanyPublicInfoDto ToDto(this Company company)
        {
            return Mapper.Map<CompanyPublicInfoDto>(company);
        }

        public static Company ToEntity(this CompanyPublicInfoDto companyJobPostDto)
        {
            return Mapper.Map<Company>(companyJobPostDto);
        }

        public static List<CompanyPublicInfoDto> ToDto(this List<Company> companyJobPosts)
        {
            return Mapper.Map<List<CompanyPublicInfoDto>>(companyJobPosts);
        }

        public static List<Company> ToEntity(this List<CompanyPublicInfoDto> companyJobPostDtos)
        {
            return Mapper.Map<List<Company>>(companyJobPostDtos);
        }

        public static PagedList<CompanyPublicInfoDto> ToDto(this PagedList<Company> companyJobPosts)
        {
            return Mapper.Map<PagedList<CompanyPublicInfoDto>>(companyJobPosts);
        }

        public static PagedList<Company> ToEntity(this PagedList<CompanyPublicInfoDto> companyJobPosts)
        {
            return Mapper.Map<PagedList<Company>>(companyJobPosts);
        }

        public static PagedResponse<CompanyPublicInfoDto> ToPagedResponse(this PagedList<CompanyPublicInfoDto> companyJobPosts)
        {
            return Mapper.Map<PagedResponse<CompanyPublicInfoDto>>(companyJobPosts);
        }
    }
}
