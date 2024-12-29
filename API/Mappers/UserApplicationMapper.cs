using API.Data.Pagination;
using API.DTOs;
using API.Entities.Applications;
using API.PaginationEntities;
using AutoMapper;
using System.Collections.Generic;

namespace API.Mappers
{
    public static class UserApplicationMapper
    {
        internal static IMapper Mapper { get; }

        static UserApplicationMapper()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<UserApplicationMapperProfile>()).CreateMapper();
        }

        public static UserApplicationDto ToDto(this UserApplication item)
        {
            return Mapper.Map<UserApplicationDto>(item);
        }

        public static UserApplication ToEntity(this UserApplicationDto item)
        {
            return Mapper.Map<UserApplication>(item);
        }

        public static List<UserApplicationDto> ToDto(this List<UserApplication> item)
        {
            return Mapper.Map<List<UserApplicationDto>>(item);
        }

        public static List<UserApplication> ToEntity(this List<UserApplicationDto> item)
        {
            return Mapper.Map<List<UserApplication>>(item);
        }

        public static PagedList<UserApplicationDto> ToDto(this PagedList<UserApplication> item)
        {
            return Mapper.Map<PagedList<UserApplicationDto>>(item);
        }

        public static PagedList<UserApplication> ToEntity(this PagedList<UserApplicationDto> item)
        {
            return Mapper.Map<PagedList<UserApplication>>(item);
        }

        public static PagedResponse<UserApplicationDto> ToPagedResponse(this PagedList<UserApplicationDto> item)
        {
            return Mapper.Map<PagedResponse<UserApplicationDto>>(item);
        }
    }
}
