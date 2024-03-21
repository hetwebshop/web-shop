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

            CreateMap<User, UserProfileDto>().ReverseMap();
            CreateMap<User, UserInfoDto>()
                .ForMember(dest => dest.PhotoUrl, act => act.MapFrom(src => src.Photo.Url));

            CreateMap<AddressDto, UserAddress>();
            CreateMap<UserAddress, AddressDto>()
                .ForMember(dest => dest.CityId, opt => opt.MapFrom(src => src.CityId))
                //.ForMember(dest => dest.StateId, opt => opt.MapFrom(src => src.City.CountryId))
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.City.CountryId));

            #endregion

            #region SeedData Entity Mapping
            #endregion
        }
    }
}
