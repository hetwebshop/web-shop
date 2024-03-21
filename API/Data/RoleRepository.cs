//using API.DTOs;
//using API.Entities;
//using API.Extensions;
//using API.Helpers;
//using API.Services;
//using AutoMapper;
//using AutoMapper.QueryableExtensions;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace API.Data
//{
//    public class RoleRepository : BaseRepository, IRoleRepository
//    {
//        private readonly UserManager<User> _userManager;

//        public RoleRepository(DataContext dataContext, IMapper mapper, IPhotoService photoService, UserManager<User> userManager) : base(dataContext, mapper, photoService)
//        {
//            _userManager = userManager;
//        }

//        public async Task<Response<BaseAgentDto, BaseRoleParams>> GetModeratorsForAdmin(int userId, BaseRoleParams roleParams)
//        {
//            if (!await IsAdmin(userId))
//                throw new HttpException("You are not Admin", StatusCodes.Status403Forbidden);

//            var rolesQuery = DataContext.UserRoles.AsQueryable();


//            if (!string.IsNullOrWhiteSpace(roleParams.UserName))
//            {
//                var inner = PredicateBuilder.False<UserRole>();
//                foreach (var name in roleParams.UserName.Split(','))
//                {
//                    inner = inner.Or(a => a.User.UserName == name.ToLower());
//                }
//                rolesQuery = rolesQuery.Where(inner);
//            }

//            rolesQuery = string.IsNullOrWhiteSpace(roleParams.Role)
//                ? rolesQuery.Where(r =>
//                    new[] { RoleType.Admin.ToString(), RoleType.StoreModerator.ToString(), RoleType.TrackModerator.ToString() }.Contains(r.Role.Name))
//                : rolesQuery.Where(r => r.Role.Name == roleParams.Role);

//            var roles = rolesQuery
//                .ProjectTo<BaseAgentDto>(Mapper.ConfigurationProvider)
//                .AsNoTracking();

//            return await Response<BaseAgentDto, BaseRoleParams>.CreateAsync(roles, roleParams);
//        }

//        public async Task<BaseAgentDto> AddModeratorByAdmin(int userId, BaseRoleDto roleDto)
//        {
//            if (!await IsAdmin(userId))
//                throw new HttpException("You are not admin", StatusCodes.Status403Forbidden);

//            if (!IsAdminRole(roleDto.Role))
//                throw new HttpException($"Invalid role - {roleDto.Role}");

//            if (await DataContext.UserRoles.AnyAsync(r => r.Role.Name == roleDto.Role && r.UserId == roleDto.UserId))
//                throw new HttpException("User already in this role");

//            var user = await DataContext.Users.Where(u => u.Id == roleDto.UserId).FirstOrDefaultAsync();

//            if (user == null)
//                throw new HttpException("User not found");

//            var result = await _userManager.AddToRoleAsync(user, roleDto.Role);
//            if (!result.Succeeded)
//                throw new HttpException(result.Errors.ToString(), StatusCodes.Status500InternalServerError);

//            return await DataContext.UserRoles
//                .Where(r => r.UserId == roleDto.UserId && r.Role.Name == roleDto.Role)
//                .ProjectTo<BaseAgentDto>(Mapper.ConfigurationProvider)
//                .FirstAsync();
//        }

//        public async Task RemoveModeratorByAdmin(int userId, BaseRoleDto roleDto)
//        {
//            if (!await IsAdmin(userId))
//                throw new HttpException("You are not admin", StatusCodes.Status403Forbidden);

//            if (!IsAdminRole(roleDto.Role))
//                throw new HttpException($"Invalid role - {roleDto.Role}");

//            if (!await DataContext.UserRoles.AnyAsync(r => r.Role.Name == roleDto.Role && r.UserId == roleDto.UserId))
//                throw new HttpException("User is not in this role");

//            var user = await DataContext.Users.Where(u => u.Id == roleDto.UserId).FirstOrDefaultAsync();

//            if (user == null)
//                throw new HttpException("User not found");

//            var result = await _userManager.RemoveFromRoleAsync(user, roleDto.Role);
//            if (!result.Succeeded)
//                throw new HttpException(result.Errors.ToString(), StatusCodes.Status500InternalServerError);
//        }


        //public async Task<List<LocationInfoDto>> SearchLocations(int userId, LocationSearchParams searchParams)
        //{
        //    IQueryable<Location> query;
        //    if (searchParams.For == RoleType.TrackModerator.ToString() || searchParams.For == RoleType.Admin.ToString())
        //        query = DataContext.Locations;
        //    else
        //        query = from agent in DataContext.TrackAgents
        //                where agent.UserId == userId && agent.Role == RoleType.TrackAdmin.ToString()
        //                join location in DataContext.Locations on agent.LocationId equals location.Id
        //                select location;

        //    return await query.Where(l => l.Name.ToLower().Contains(searchParams.Name.ToLower()) && l.Type == searchParams.Type)
        //        .Take(16)
        //        .ProjectTo<LocationInfoDto>(Mapper.ConfigurationProvider)
        //        .ToListAsync();
        //}

        //public async Task<List<StoreInfoDto>> SearchStores(int userId, StoreSearchParams searchParams)
        //{
        //    IQueryable<Store> query;
        //    if (searchParams.For == RoleType.StoreModerator.ToString() || searchParams.For == RoleType.Admin.ToString())
        //        query = DataContext.Stores;
        //    else
        //        query = from agent in DataContext.StoresAgents
        //                where agent.UserId == userId && agent.Role == RoleType.StoreAdmin.ToString()
        //                join store in DataContext.Stores on agent.StoreId equals store.Id
        //                select store;

        //    return await query.Where(s => s.Name.ToLower().Contains(searchParams.Name.ToLower()))
        //        .Take(16)
        //        .ProjectTo<StoreInfoDto>(Mapper.ConfigurationProvider)
        //        .ToListAsync();
        //}
//    }
//}
