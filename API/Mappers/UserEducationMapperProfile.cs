using API.DTOs;
using API.Entities;
using AutoMapper;

namespace API.Mappers
{
    public class UserEducationMapperProfile : Profile
    {
        public UserEducationMapperProfile()
        {
            this.CreateMap<UserEducation, UserEducationDto>()
                .ForMember(r => r.UserId, src => src.MapFrom(x => x.UserId))
                .ForMember(r => r.EducationStartYear, src => src.MapFrom(x => x.EducationStartYear))
                .ForMember(r => r.EducationEndYear, src => src.MapFrom(x => x.EducationEndYear))
                .ForMember(r => r.FieldOfStudy, src => src.MapFrom(x => x.FieldOfStudy))
                .ForMember(r => r.Degree, src => src.MapFrom(x => x.Degree))
                .ForMember(r => r.InstitutionName, src => src.MapFrom(x => x.InstitutionName))
                .ForMember(r => r.University, src => src.MapFrom(x => x.University)).ReverseMap();
        }
    }
}
