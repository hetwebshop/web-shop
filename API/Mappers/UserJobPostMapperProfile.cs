﻿using API.Data.Pagination;
using API.DTOs;
using API.Entities;
using API.Entities.JobPost;
using API.Helpers;
using API.PaginationEntities;
using AutoMapper;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace API.Mappers
{
    public class UserJobPostMapperProfile : Profile
    {
        public UserJobPostMapperProfile()
        {
            this.CreateMap<UserJobPost, UserJobPostDto>()
                .ForMember(dest => dest.JobCategory, src => src.MapFrom(x => x.JobCategory.Name))
                .ForMember(dest => dest.JobType, src => src.MapFrom(x => x.JobType.Name))
                .ForMember(dest => dest.JobPostStatus, src => src.MapFrom(x => x.JobPostStatus.Name))
                .ForMember(dest => dest.City, src => src.MapFrom(x => x.City.Name))
                .ForMember(dest => dest.CityId, src => src.MapFrom(x => x.CityId))
                .ForMember(dest => dest.ApplicantGender, src => src.MapFrom(x => x.ApplicantGender))
                .ForMember(dest => dest.EmploymentType, src => src.MapFrom(x => x.EmploymentType.Name))
                .ForMember(dest => dest.EmploymentStatus, src => src.MapFrom(x => x.EmploymentStatus.Name))
                .ForMember(dest => dest.EducationLevel, src => src.MapFrom(x => x.EducationLevel.Name))
                .ForMember(dest => dest.ApplicantPreviousCompanies, src => src.MapFrom(x =>
                    x.ApplicantPreviousCompanies.Select(r => new ApplicantPreviousCompaniesDto
                    {
                        UserCompanyId = r.Id,
                        CompanyName = r.CompanyName,
                        Description = r.Description,
                        StartYear = r.StartYear,
                        EndYear = r.EndYear,
                        Position = r.Position
                    }).ToList() ?? new List<ApplicantPreviousCompaniesDto>()))
               .ForMember(dest => dest.ApplicantEducations, src => src.MapFrom(x =>
                    x.ApplicantEducations.Select(r => new ApplicantEducationDto
                    {
                        Degree = r.Degree,
                        EducationEndYear = r.EducationEndYear,
                        EducationStartYear = r.EducationStartYear,
                        FieldOfStudy = r.FieldOfStudy,
                        University = r.University,
                        InstitutionName = r.University,
                        UserEducationId = r.Id
                    }).ToList() ?? new List<ApplicantEducationDto>()))
                .ForMember(dest => dest.AdvertisementTypeId, src => src.MapFrom(x => x.AdvertisementTypeId))
                .ForMember(dest => dest.CvFilePath, src => src.MapFrom(x => x.CvFilePath))
                .ForMember(dest => dest.PricingPlanName, src => src.MapFrom(x => x.PricingPlan.Name))
                .ForMember(dest => dest.AdDuration, src => src.MapFrom(x => x.PricingPlan.AdActiveDays));

            this.CreateMap<UserJobPostDto, UserJobPost>()
                .ForMember(dest => dest.JobPostStatusId, src => src.MapFrom(x => x.JobPostStatusId != 0 ? x.JobPostStatusId : (int)Helpers.JobPostStatus.Active))
                .ForMember(dest => dest.PricingPlan, opt => opt.Ignore());

            this.CreateMap<ApplicantEducation, ApplicantEducationDto>();
            this.CreateMap<ApplicantEducationDto, ApplicantEducation>();

            this.CreateMap<ApplicantPreviousCompanies, ApplicantPreviousCompaniesDto>();
            this.CreateMap<ApplicantPreviousCompaniesDto, ApplicantPreviousCompanies>();

            CreateMap(typeof(PagedList<>), typeof(PagedList<>)).ConvertUsing(typeof(PagedListConverter<,>));

            this.CreateMap<PagedList<UserJobPostDto>, PagedResponse<UserJobPostDto>>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.ToList()))
                .ForMember(dest => dest.CurrentPage, opt => opt.MapFrom(src => src.CurrentPage))
                .ForMember(dest => dest.TotalPages, opt => opt.MapFrom(src => src.TotalPages))
                .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.PageSize))
                .ForMember(dest => dest.TotalCount, opt => opt.MapFrom(src => src.TotalCount))
                .ForMember(dest => dest.HasPrevious, opt => opt.MapFrom(src => src.HasPrevious))
                .ForMember(dest => dest.HasNext, opt => opt.MapFrom(src => src.HasNext));
            
            //this.CreateMap<PagedList<UserJobPostDto>, PagedResponse<UserJobPostDto>>()
            //    .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.ToList()));
        }

        public class PagedListConverter<TSource, TDestination> : ITypeConverter<PagedList<TSource>, PagedList<TDestination>>
        {
            public PagedList<TDestination> Convert(PagedList<TSource> source, PagedList<TDestination> destination, ResolutionContext context)
            {
                var mappedItems = context.Mapper.Map<IEnumerable<TSource>, IEnumerable<TDestination>>(source);
                return new PagedList<TDestination>(mappedItems.ToList(), source.TotalCount, source.CurrentPage, source.PageSize);
            }
        }

        //private string MapGender(Gender gender)
        //{
        //    switch (gender)
        //    {
        //        case Gender.Male:
        //            return "Muško";
        //        case Gender.Female:
        //            return "Žensko";
        //        default:
        //            return "Ostali";
        //    }
        //}

        //private List<UserJobSubcategory> ConvertToUserJobSubcategories(int userJobPostId, List<int> subcategories)
        //{

        //}
    }
}
