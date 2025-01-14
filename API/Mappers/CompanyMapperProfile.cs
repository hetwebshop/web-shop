using API.Data.Pagination;
using API.DTOs;
using API.Entities;
using API.PaginationEntities;
using AutoMapper;
using System.Linq;
using static API.Mappers.UserApplicationMapperProfile;

namespace API.Mappers
{
    public class CompanyMapperProfile : Profile
    {
        public CompanyMapperProfile()
        {
            this.CreateMap<Company, CompanyPublicInfoDto>()
                .ForMember(r => r.City, x => x.MapFrom(k => k.City.Name));

            this.CreateMap<CompanyPublicInfoDto, Company>();


            CreateMap(typeof(PagedList<>), typeof(PagedList<>)).ConvertUsing(typeof(PagedListConverter<,>));

            this.CreateMap<PagedList<CompanyPublicInfoDto>, PagedResponse<CompanyPublicInfoDto>>()
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
