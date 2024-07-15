using API.DTOs;
using API.Entities;
using API.Seed;
using AutoMapper;
using System;
using System.Linq;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            #region Entity DTO Mapping

            CreateMap<UserRegisterDto, User>()
                .ForMember(dest => dest.UserName,
                    opt => opt.MapFrom(src => src.UserName.ToLower()));

            CreateMap<CompanyRegisterDto, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName.ToLower()))
                .ForMember(dest => dest.IsCompany, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Company, opt => opt.MapFrom(src => new Company
                {
                    CompanyName = src.CompanyName,
                    CityId = src.CityId,
                    Address = src.Address,
                    Email = src.Email,
                    PhoneNumber = src.PhoneNumber,
                }));

            CreateMap<User, UserProfileDto>()
                .ForMember(dest => dest.CityId, opt => opt.MapFrom(src => src.CityId))
                .ForMember(dest => dest.CompanyAddress, opt => opt.MapFrom(src => src.Company.Address))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.CompanyName))
                .ForMember(dest => dest.CompanyPhone, opt => opt.MapFrom(src => src.Company.PhoneNumber))
                .ForMember(dest => dest.AboutCompany, opt => opt.MapFrom(src => src.Company.AboutUs))
                .ReverseMap();

            CreateMap<UserEducation, UserEducationDto>().ReverseMap();

            CreateMap<User, UserInfoDto>()
                .ForMember(dest => dest.PhotoUrl, act => act.MapFrom(src => src.Photo.Url));

            #endregion

            #region SeedData Entity Mapping
            #endregion
        }
    }
}
