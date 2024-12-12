using API.DTOs;
using API.Entities;
using API.Seed;
using AutoMapper;
using System;
using System.IO;
using System.Linq;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
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
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(dest => dest.CompanyAddress, opt => opt.MapFrom(src => src.Company.Address))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.CompanyName))
                .ForMember(dest => dest.CompanyPhone, opt => opt.MapFrom(src => src.Company.PhoneNumber))
                .ForMember(dest => dest.AboutCompany, opt => opt.MapFrom(src => src.Company.AboutUs))
                .ForMember(dest => dest.CvFilePath, opt => opt.MapFrom(src => Path.GetFileName(src.CvFilePath)))
                .ReverseMap();

            CreateMap<UserProfileDto, User>()
                .ForMember(dest => dest.CityId, opt => opt.MapFrom(src => src.CityId))
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyId));


            CreateMap<UserEducation, UserEducationDto>().ReverseMap();

            CreateMap<User, UserInfoDto>()
                .ForMember(dest => dest.PhotoUrl, act => act.MapFrom(src => src.Photo.Url));
        }
    }
}
