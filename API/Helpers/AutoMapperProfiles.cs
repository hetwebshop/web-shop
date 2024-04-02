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

            CreateMap<User, UserProfileDto>()
                .ForMember(dest => dest.CityId, opt => opt.MapFrom(src => src.CityId))
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
