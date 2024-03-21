using API.DTOs;
using API.Entities;
using API.Entities.JobPost;
using API.Helpers;
using AutoMapper;
using System.Collections.Generic;
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
                .ForMember(dest => dest.Country, src => src.MapFrom(x => x.City.Country.Name))
                .ForMember(dest => dest.CityId, src => src.MapFrom(x => x.City.Id))
                .ForMember(dest => dest.CountryId, src => src.MapFrom(x => x.City.Country.Id))
                .ForMember(dest => dest.ApplicantGender, src => src.MapFrom(x => MapGender(x.ApplicantGender)))
                .ForMember(dest => dest.ApplicantEducations, src => src.MapFrom(x => x.ApplicantEducations))
                .ForMember(dest => dest.AdvertisementTypeId, src => src.MapFrom(x => x.AdvertisementTypeId))
                .ForMember(dest => dest.UserJobSubcategories, src => src.MapFrom(x => x.UserJobSubcategories));

            this.CreateMap<UserJobPostDto, UserJobPost>()
                .ForMember(dest => dest.JobPostStatusId, src => src.MapFrom(x => x.JobPostStatusId != 0 ? x.JobPostStatusId : (int)Helpers.JobPostStatus.Active));

            this.CreateMap<ApplicantEducation, ApplicantEducationDto>();
            this.CreateMap<ApplicantEducationDto, ApplicantEducation>();

            this.CreateMap<UserJobSubcategory, UserJobSubcategoryDto>();
            this.CreateMap<UserJobSubcategoryDto, UserJobSubcategory>();
        }

        private string MapGender(Gender gender)
        {
            switch (gender)
            {
                case Gender.Male:
                    return "Male";
                case Gender.Female:
                    return "Female";
                default:
                    return "Other";
            }
        }

        //private List<UserJobSubcategory> ConvertToUserJobSubcategories(int userJobPostId, List<int> subcategories)
        //{

        //}
    }
}
