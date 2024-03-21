using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Services;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public class BaseRepository
    {
        public DataContext DataContext { get; }
        public IMapper Mapper { get; }
        public IPhotoService PhotoService { get; }

        public BaseRepository(DataContext dataContext, IMapper mapper, IPhotoService photoService)
        {
            DataContext = dataContext;
            Mapper = mapper;
            PhotoService = photoService;
        }

        #region Save

        public async Task<bool> SaveChanges()
        {
            return await DataContext.SaveChangesAsync() > 0;
        }

        public bool HasChanges()
        {
            return DataContext.ChangeTracker.HasChanges();
        }

        #endregion

        #region User

        public async Task<string> GetUserNameById(int id)
        {
            return await DataContext.Users.Where(u => u.Id == id)
                .Select(u => u.UserName)
                .SingleOrDefaultAsync();
        }

        public async Task<int> GetUserIdByUserName(string userName)
        {
            return await DataContext.Users.Where(u => u.UserName == userName.ToLower())
                .Select(u => u.Id)
                .SingleOrDefaultAsync();
        }

        public async Task<bool> UserExist(int id)
        {
            return await DataContext.Users.AnyAsync(u => u.Id == id);
        }

        public async Task<bool> UserExist(string userName)
        {
            return await DataContext.Users.AnyAsync(u => u.UserName == userName.ToLower());
        }

        public async Task<UserInfoDto> GetUserInfo(string userName)
        {
            var user = await DataContext.Users.Where(u => u.UserName == userName.ToLower())
                .ProjectTo<UserInfoDto>(Mapper.ConfigurationProvider).FirstOrDefaultAsync();

            if (user == null)
                user = new UserInfoDto();
            else
                user.Exist = true;

            return user;
        }

        public async Task<bool> UserHasAddress(int userId)
        {
            if (userId == 0) return false;
            return await DataContext.Users.AnyAsync(u => u.AddressId != null);
        }

        public async Task<UserAddress> GetUserAddress(int userId)
        {
            return await DataContext.UserAddresses.Where(a => a.User.Id == userId).FirstOrDefaultAsync();
        }

        public async Task<string> GetUserAddressName(int userId)
        {
            if (userId == 0) return null;
            return await DataContext.Users
                .Where(u => u.Id == userId)
                .Select(u => u.Address.StreetName)
                .FirstOrDefaultAsync();
        }

        #endregion

        #region Photo

        public async Task UpdatePhoto(IFormFile file, Photo photo, bool profile = false)
        {
            var result = await PhotoService.UploadImage(file, profile);
            if (result.Error != null) throw new HttpException(result.Error.Message, StatusCodes.Status500InternalServerError);
            photo.PublicId = result.PublicId;
            photo.Url = result.SecureUrl.AbsoluteUri;
        }

        public async Task DeletePhoto(Photo photo)
        {
            await PhotoService.DeleteImage(photo.PublicId);
            photo.PublicId = null;
            photo.Url = null;
        }

        public async Task DeletePhoto(string publicId)
        {
            await PhotoService.DeleteImage(publicId);
        }

        #endregion

        #region Role

        public async Task<bool> IsTrackModerator(int userId)
        {
            return await DataContext.UserRoles.Where(r => new[] { RoleType.TrackModerator.ToString(), RoleType.Admin.ToString() }
                    .Contains(r.Role.Name) && r.UserId == userId)
                .AnyAsync();
        }

        public async Task<bool> IsStoreModerator(int userId)
        {
            return await DataContext.UserRoles.Where(r => new[] { RoleType.StoreModerator.ToString(), RoleType.Admin.ToString() }
                    .Contains(r.Role.Name) && r.UserId == userId)
                .AnyAsync();
        }

        public async Task<bool> IsAdmin(int userId)
        {
            return await DataContext.UserRoles.Where(r => r.Role.Name == RoleType.Admin.ToString() && r.UserId == userId)
                .AnyAsync();
        }

        public bool IsStoreRole(string role)
        {
            return new[] { RoleType.StoreAdmin.ToString(), RoleType.StoreAgent.ToString() }.Contains(role);
        }

        public bool IsTrackRole(string role)
        {
            return new[] { RoleType.TrackAdmin.ToString(), RoleType.TrackAgent.ToString() }.Contains(role);
        }

        public bool IsAdminRole(string role)
        {
            return new[] { RoleType.StoreModerator.ToString(), RoleType.TrackModerator.ToString(), RoleType.Admin.ToString() }.Contains(role);
        }

        #endregion
    }
}
