using API.Data.Pagination;
using API.DTOs;
using API.Entities;
using API.Entities.Applications;
using API.PaginationEntities;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;

namespace API.Mappers
{
    public class UserApplicationMapperProfile : Profile
    {
        public UserApplicationMapperProfile()
        {
            this.CreateMap<UserApplication, UserApplicationDto>()
                .ForMember(dest => dest.City, src => src.MapFrom(x => x.City.Name))
                .ForMember(dest => dest.CityId, src => src.MapFrom(x => x.CityId))
                .ForMember(dest => dest.Gender, src => src.MapFrom(x => x.Gender))
                .ForMember(dest => dest.PreviousCompanies, src => src.MapFrom(x =>
                    x.PreviousCompanies.Select(r => new UserPreviousCompaniesDto
                    {
                        UserCompanyId = r.Id,
                        CompanyName = r.CompanyName,
                        Description = r.Description,
                        StartYear = r.StartYear,
                        EndYear = r.EndYear,
                        Position = r.Position
                    }).ToList() ?? new List<UserPreviousCompaniesDto>()))
               .ForMember(dest => dest.Educations, src => src.MapFrom(x =>
                    x.Educations.Select(r => new UserEducationDto
                    {
                        Degree = r.Degree,
                        EducationEndYear = r.EducationEndYear,
                        EducationStartYear = r.EducationStartYear,
                        FieldOfStudy = r.FieldOfStudy,
                        University = r.University,
                        InstitutionName = r.University,
                        UserEducationId = r.Id
                    }).ToList() ?? new List<UserEducationDto>()))
                .ForMember(dest => dest.CvFilePath, src => src.MapFrom(x => x.CvFilePath))
                .ForMember(dest => dest.CvFileName, src => src.MapFrom(x => x.CvFileName));

            this.CreateMap<UserApplicationDto, UserApplication>()
                .ForMember(dest => dest.CityId, src => src.MapFrom(x => x.CityId))
                .ForMember(dest => dest.Gender, src => src.MapFrom(x => x.Gender))
                .ForMember(dest => dest.PreviousCompanies, src => src.MapFrom(x =>
                    x.PreviousCompanies.Select(r => new UserApplicationPreviousCompanies
                    {
                        CompanyName = r.CompanyName,
                        Description = r.Description,
                        StartYear = r.StartYear,
                        EndYear = r.EndYear,
                        Position = r.Position
                    }).ToList() ?? new List<UserApplicationPreviousCompanies>()))
               .ForMember(dest => dest.Educations, src => src.MapFrom(x =>
                    x.Educations.Select(r => new UserApplicationEducation
                    {
                        Degree = r.Degree,
                        EducationEndYear = r.EducationEndYear,
                        EducationStartYear = r.EducationStartYear,
                        FieldOfStudy = r.FieldOfStudy,
                        University = r.University,
                        InstitutionName = r.University,
                    }).ToList() ?? new List<UserApplicationEducation>()))
                .ForMember(dest => dest.CvFilePath, src => src.MapFrom(x => x.CvFilePath));

            this.CreateMap<UserApplicationEducation, UserEducationDto>();
            this.CreateMap<UserEducationDto, UserApplicationEducation>();

            this.CreateMap<UserPreviousCompanies, UserPreviousCompaniesDto>();
            this.CreateMap<UserPreviousCompaniesDto, UserPreviousCompanies>();

            this.CreateMap<UserApplicationPreviousCompanies, UserPreviousCompaniesDto>();
            this.CreateMap<UserPreviousCompaniesDto, UserApplicationPreviousCompanies>();

            CreateMap(typeof(PagedList<>), typeof(PagedList<>)).ConvertUsing(typeof(PagedListConverter<,>));

            this.CreateMap<PagedList<UserApplicationDto>, PagedResponse<UserApplicationDto>>()
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
    }
}
