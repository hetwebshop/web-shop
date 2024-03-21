using API.DTOs;
using API.Entities.JobPost;
using AutoMapper;
using System.Collections.Generic;

namespace API.Mappers
{
    public static class UserJobPostMapper
    {
        internal static IMapper Mapper { get; }

        static UserJobPostMapper()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<UserJobPostMapperProfile>()).CreateMapper();
        }

        public static UserJobPostDto ToDto(this UserJobPost userJobPost)
        {
            return Mapper.Map<UserJobPostDto>(userJobPost);
        }

        public static UserJobPost ToEntity(this UserJobPostDto userJobPostDto)
        {
            return Mapper.Map<UserJobPost>(userJobPostDto);
        }

        public static List<UserJobPostDto> ToDto(this List<UserJobPost> userJobPosts)
        {
            return Mapper.Map<List<UserJobPostDto>>(userJobPosts);
        }

        public static List<UserJobPost> ToEntity(this List<UserJobPostDto> userJobPostDtos)
        {
            return Mapper.Map<List<UserJobPost>>(userJobPostDtos);
        }
    }
}
