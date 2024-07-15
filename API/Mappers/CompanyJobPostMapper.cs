using API.Data.Pagination;
using API.DTOs;
using API.Entities.CompanyJobPost;
using API.PaginationEntities;
using AutoMapper;
using System.Collections.Generic;

namespace API.Mappers
{
    public static class CompanyJobPostMapper
    {
        internal static IMapper Mapper { get; }

        static CompanyJobPostMapper()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<CompanyJobPostMapperProfile>()).CreateMapper();
        }

        public static CompanyJobPostDto ToDto(this CompanyJobPost companyJobPost)
        {
            return Mapper.Map<CompanyJobPostDto>(companyJobPost);
        }

        public static CompanyJobPost ToEntity(this CompanyJobPostDto companyJobPostDto)
        {
            return Mapper.Map<CompanyJobPost>(companyJobPostDto);
        }

        public static List<CompanyJobPostDto> ToDto(this List<CompanyJobPost> companyJobPosts)
        {
            return Mapper.Map<List<CompanyJobPostDto>>(companyJobPosts);
        }

        public static List<CompanyJobPost> ToEntity(this List<CompanyJobPostDto> companyJobPostDtos)
        {
            return Mapper.Map<List<CompanyJobPost>>(companyJobPostDtos);
        }

        public static PagedList<CompanyJobPostDto> ToDto(this PagedList<CompanyJobPost> companyJobPosts)
        {
            return Mapper.Map<PagedList<CompanyJobPostDto>>(companyJobPosts);
        }

        public static PagedList<CompanyJobPost> ToEntity(this PagedList<CompanyJobPostDto> companyJobPosts)
        {
            return Mapper.Map<PagedList<CompanyJobPost>>(companyJobPosts);
        }

        public static PagedResponse<CompanyJobPostDto> ToPagedResponse(this PagedList<CompanyJobPostDto> companyJobPosts)
        {
            return Mapper.Map<PagedResponse<CompanyJobPostDto>>(companyJobPosts);
        }
    }
}
