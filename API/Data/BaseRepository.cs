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

        public BaseRepository(DataContext dataContext, IMapper mapper)
        {
            DataContext = dataContext;
            Mapper = mapper;
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

        #endregion
    }
}
