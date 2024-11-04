using API.Data.Pagination;
using API.DTOs;
using API.Entities;
using API.Entities.CompanyJobPost;
using API.PaginationEntities;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using static API.Mappers.UserJobPostMapperProfile;

namespace API.Mappers
{
    public class CompanyJobPostMapperProfile : Profile
    {
        public CompanyJobPostMapperProfile()
        {
            this.CreateMap<CompanyJobPost, CompanyJobPostDto>()
                .ForMember(dest => dest.JobCategory, src => src.MapFrom(x => x.JobCategory.Name))
                .ForMember(dest => dest.JobType, src => src.MapFrom(x => x.JobType.Name))
                .ForMember(dest => dest.JobPostStatus, src => src.MapFrom(x => x.JobPostStatus.Name))
                .ForMember(dest => dest.City, src => src.MapFrom(x => x.City.Name))
                .ForMember(dest => dest.CityId, src => src.MapFrom(x => x.CityId))
                .ForMember(dest => dest.PricingPlanName, src => src.MapFrom(x => x.PricingPlan.Name))
                .ForMember(dest => dest.AdDuration, src => src.MapFrom(x => x.PricingPlan.AdActiveDays));

            this.CreateMap<CompanyJobPostDto, CompanyJobPost>()
                .ForMember(dest => dest.JobPostStatusId, src => src.MapFrom(x => x.JobPostStatusId != 0 ? x.JobPostStatusId : (int)Helpers.JobPostStatus.Active)).ForMember(dest => dest.PricingPlan, src => src.MapFrom(x => new PricingPlan { AdActiveDays = x.AdDuration, Name = x.PricingPlanName }))
                .ForMember(dest => dest.PricingPlan, src => src.MapFrom(x => new PricingPlan { AdActiveDays = x.AdDuration, Name = x.PricingPlanName }));


            CreateMap(typeof(PagedList<>), typeof(PagedList<>)).ConvertUsing(typeof(PagedListConverter<,>));

            this.CreateMap<PagedList<CompanyJobPostDto>, PagedResponse<CompanyJobPostDto>>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.ToList()))
                .ForMember(dest => dest.CurrentPage, opt => opt.MapFrom(src => src.CurrentPage))
                .ForMember(dest => dest.TotalPages, opt => opt.MapFrom(src => src.TotalPages))
                .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.PageSize))
                .ForMember(dest => dest.TotalCount, opt => opt.MapFrom(src => src.TotalCount))
                .ForMember(dest => dest.HasPrevious, opt => opt.MapFrom(src => src.HasPrevious))
                .ForMember(dest => dest.HasNext, opt => opt.MapFrom(src => src.HasNext));
        }
    }
}
