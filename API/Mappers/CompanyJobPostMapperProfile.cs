using API.Data.Pagination;
using API.DTOs;
using API.Entities;
using API.Entities.CompanyJobPost;
using API.Entities.JobPost;
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
                .ForMember(dest => dest.EducationLevel, src => src.MapFrom(x => x.EducationLevel.Name))
                .ForMember(dest => dest.EmploymentType, src => src.MapFrom(x => x.EmploymentType.Name))
                .ForMember(dest => dest.AdDuration, src => src.MapFrom(x => x.PricingPlan.AdActiveDays))
                .ForMember(dest => dest.JobCategoryId, src => src.MapFrom(x => x.JobCategoryId))
                .ForMember(dest => dest.JobTypeId, src => src.MapFrom(x => x.JobTypeId))
                .ForMember(dest => dest.UsersThatAppliedOnJobPost, src => src.MapFrom(x => x.UserApplications.Select(r => r.SubmittingUserId).ToList()))
                .ForMember(dest => dest.EmailForReceivingApplications, src => src.MapFrom(x => x.EmailForReceivingApplications))
               .ForMember(dest => dest.SalaryRange, src => src.MapFrom(x => x.MinSalary != null && x.MaxSalary != null ? new[] { x.MinSalary, x.MaxSalary } : null));

            this.CreateMap<CompanyJobPostDto, CompanyJobPost>()
                //.ForMember(dest => dest.City, src => src.MapFrom(x => new City { Id = x.CityId, Name = x.City }))
                //.ForMember(dest => dest.JobPostStatus, src => src.MapFrom(x => new JobPostStatus { Id = x.JobPostStatusId != 0 ? x.JobPostStatusId : (int)Helpers.JobPostStatus.Active }))
                .ForMember(dest => dest.JobPostStatusId, src => src.MapFrom(x => x.JobPostStatusId != 0 ? x.JobPostStatusId : (int)Helpers.JobPostStatus.Active))
                //.ForMember(dest => dest.JobType, src => src.MapFrom(x => new JobType { Name = x.JobType, Id = x.JobTypeId }))
                //.ForMember(dest => dest.JobCategory, src => src.MapFrom(x => new JobCategory { Name = x.JobCategory, Id = x.JobCategoryId }))
                .ForMember(dest => dest.JobType, opt => opt.Ignore())
                .ForMember(dest => dest.JobCategory, opt => opt.Ignore())
                .ForMember(dest => dest.JobPostStatus, opt => opt.Ignore())
                .ForMember(dest => dest.City, opt => opt.Ignore())
                .ForMember(dest => dest.EmploymentType, opt => opt.Ignore())
                .ForMember(dest => dest.EducationLevel, opt => opt.Ignore())
                .ForMember(dest => dest.PricingPlan, opt => opt.Ignore())
                .ForMember(dest => dest.PricingPlan, src => src.MapFrom(x => new PricingPlanCompanies { AdActiveDays = x.AdDuration, Name = x.PricingPlanName }))
                .ForMember(dest => dest.MaxSalary, src => src.MapFrom(x => x.SalaryRange != null && x.SalaryRange.Length > 1 ? x.SalaryRange[1] : (int?)null))
                .ForMember(dest => dest.MinSalary, src => src.MapFrom(x => x.SalaryRange != null && x.SalaryRange.Length > 0 ? x.SalaryRange[0] : (int?)null));


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
