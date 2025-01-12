using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Services;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(DataContext dataContext, IMapper mapper) : base(dataContext, mapper)
        {

        }

        public async Task<UserProfileDto> GetProfile(int id)
        {
            var user = await DataContext.Users
                .Include(u => u.City)
                .Include(u => u.UserEducations)
                .Include(u => u.Company)
                .Where(u => u.Id == id)
                .ProjectTo<UserProfileDto>(Mapper.ConfigurationProvider)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            return user;
        }

        public async Task<string> UpdateUserPhoto(IFormFile file, int userId)
        {
            var photo = await DataContext.Photos.FirstOrDefaultAsync(p => p.User.Id == userId) ?? new Photo
            {
                User = new User { Id = userId }
            };
            var publicId = photo.PublicId;
            //await UpdatePhoto(file, photo, true);
            if (!await SaveChanges())
                throw new HttpException("Failed to update photo.", StatusCodes.Status500InternalServerError);

            //await DeletePhoto(publicId);
            return photo.Url;
        }

        public async Task DeleteUserPhoto(int userId)
        {
            var photo = await DataContext.Photos.FirstOrDefaultAsync(p => p.User.Id == userId);
            if (photo?.Url == null) throw new HttpException("Photo already removed!");
            //await DeletePhoto(photo);
        }

        public async Task<List<UserEducation>> GetAllUserEducationsAsync(int userId)
        {
            var userEducations = await DataContext.UserEducations.Where(u => u.UserId == userId).ToListAsync();
            return userEducations;
        }

        public async Task<bool> RemoveAllUserEducationsAsync(int userId)
        {
            var userEducations = await GetAllUserEducationsAsync(userId);
            if (userEducations.Any())
            {
                DataContext.UserEducations.RemoveRange(userEducations);
                if(await SaveChanges())
                    return true;
            }
            return false;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return DataContext.Users.Where(r => r.Id == userId).FirstOrDefault();
        }

        public async Task<bool> UpdateUserEducationAsync(UserEducationRequest userEducation)
        {
            var existingEducation = DataContext.UserEducations.First(r => r.Id == userEducation.UserEducationId);
            if (existingEducation == null)
                return false;
            existingEducation.EducationStartYear = userEducation.EducationStartYear;
            existingEducation.EducationEndYear = userEducation.EducationEndYear;
            existingEducation.University = userEducation.University;
            existingEducation.InstitutionName = userEducation.InstitutionName;
            existingEducation.FieldOfStudy = userEducation.FieldOfStudy;
            existingEducation.Degree = userEducation.Degree;
            await SaveChanges();
            return true;
        }

        public async Task<bool> UpdateUserPreviousCompaniesAsync(UserCompanyRequest req)
        {
            var existing = DataContext.UserPreviousCompanies.First(r => r.Id == req.UserCompanyId);
            if (existing == null)
                return false;
            existing.CompanyName = req.CompanyName;
            existing.Position = req.Position;
            existing.Description = req.Description;
            existing.StartYear = req.StartYear;
            existing.EndYear = req.EndYear;
            await SaveChanges();
            return true;
        }
    }
}
