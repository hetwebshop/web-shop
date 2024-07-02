using API.DTOs;
using API.Entities;
using AutoMapper;
using System.Collections.Generic;

namespace API.Mappers
{
    public static class UserEducationMapper
    {
        internal static IMapper Mapper { get; }

        static UserEducationMapper()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<UserEducationMapperProfile>()).CreateMapper();
        }

        public static List<UserEducationDto> ToDto(this List<UserEducation> userEducations)
        {
            return Mapper.Map<List<UserEducationDto>>(userEducations);
        }

        public static List<UserEducation> ToEntity(this List<UserEducationDto> userEducationsDto)
        {
            return Mapper.Map<List<UserEducation>>(userEducationsDto);
        }
    }
}
