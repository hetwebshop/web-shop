using API.Data;
using API.Entities;
using API.Entities.JobPost;
using API.Helpers;
using AutoMapper;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using JobPostStatus = API.Entities.JobPost.JobPostStatus;

namespace API.Seed
{
    public class SeedData
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IConfiguration _config;
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private static readonly Random Random = new();

        public SeedData(UserManager<User> userManager, RoleManager<Role> roleManager,
            IConfiguration config, DataContext context, IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
            _context = context;
            _mapper = mapper;
        }

        public async Task SeedDatabase()
        {
            await _context.Database.MigrateAsync();
            await SeedCountriesCitiesEtc();
            await SeedRoles();
            await SeedCompanies();
            await SeedUsers();

            //JobPost
            await SeedUserJobPostModels();
        }

        async Task SeedUserJobPostModels()
        {
            await SeedJobTypes();
            await SeedJobPostStatuses();
            await SeedJobCategoriesAndSubcategories();
            await SeedAdvertisementTypes();
            await SeedEmploymentTypes();
            await SeedEducationLevels();
            await SeedEmploymentStatuses();
            await SeedPricingPlans();
            //await SeedUserJobPosts();
        }

        async Task SeedCompanies()
        {
            if (await _context.Companies.AnyAsync()) return;

            var data = await File.ReadAllTextAsync("Seed/CompanySeed.json");
            var deserialized = JsonSerializer.Deserialize<List<Company>>(data);

            await _context.Companies.AddRangeAsync(deserialized);

            await SetIdentityInsertAndSaveChanges("Companies");
        }

        async Task SeedAdvertisementTypes()
        {
            if (await _context.AdvertisementTypes.AnyAsync()) return;

            var data = await File.ReadAllTextAsync("Seed/AdvertisementTypeSeed.json");
            var deserialized = JsonSerializer.Deserialize<List<AdvertisementType>>(data);

            await _context.AdvertisementTypes.AddRangeAsync(deserialized);

            await SetIdentityInsertAndSaveChanges("AdvertisementTypes");
        }

        async Task SeedEmploymentTypes()
        {
            if (await _context.EmploymentTypes.AnyAsync()) return;

            var data = await File.ReadAllTextAsync("Seed/EmploymentTypeSeed.json");
            var deserialized = JsonSerializer.Deserialize<List<EmploymentType>>(data);

            await _context.EmploymentTypes.AddRangeAsync(deserialized);

            await SetIdentityInsertAndSaveChanges("EmploymentTypes");
        }

        async Task SeedEducationLevels()
        {
            if (await _context.EducationLevels.AnyAsync()) return;

            var data = await File.ReadAllTextAsync("Seed/EducationLevelSeed.json");
            var deserialized = JsonSerializer.Deserialize<List<EducationLevel>>(data);

            await _context.EducationLevels.AddRangeAsync(deserialized);

            await SetIdentityInsertAndSaveChanges("EducationLevels");
        }

        async Task SeedEmploymentStatuses()
        {
            if (await _context.EmploymentStatuses.AnyAsync()) return;

            var data = await File.ReadAllTextAsync("Seed/EmploymentStatus.json");
            var deserialized = JsonSerializer.Deserialize<List<EmploymentStatus>>(data);

            await _context.EmploymentStatuses.AddRangeAsync(deserialized);

            await SetIdentityInsertAndSaveChanges("EmploymentStatuses");
        }

        async Task SeedPricingPlans()
        {
            if (await _context.PricingPlans.AnyAsync()) return;

            var data = await File.ReadAllTextAsync("Seed/PricingPlanSeed.json");
            var deserialized = JsonSerializer.Deserialize<List<PricingPlan>>(data);

            await _context.PricingPlans.AddRangeAsync(deserialized);

            await SetIdentityInsertAndSaveChanges("PricingPlans");
        }

        async Task SeedJobTypes()
        {
            if (await _context.JobTypes.AnyAsync()) return;

            var data = await File.ReadAllTextAsync("Seed/JobTypeSeed.json");
            var jobTypes = JsonSerializer.Deserialize<List<JobType>>(data);

            await _context.JobTypes.AddRangeAsync(jobTypes);

            await SetIdentityInsertAndSaveChanges("JobTypes");
        }

        async Task SeedJobPostStatuses()
        {
            if (await _context.JobPostStatuses.AnyAsync()) return;

            var data = await File.ReadAllTextAsync("Seed/JobStatusSeed.json");
            var jobStatuses = JsonSerializer.Deserialize<List<JobPostStatus>>(data);

            await _context.JobPostStatuses.AddRangeAsync(jobStatuses);

            await SetIdentityInsertAndSaveChanges("JobPostStatuses");
        }

        async Task SeedJobCategoriesAndSubcategories()
        {
            if (await _context.JobCategories.AnyAsync()) return;

            var data = await File.ReadAllTextAsync("Seed/JobCategorySeed.json");
            var jobCategories = JsonSerializer.Deserialize<List<JobCategory>>(data);

            await _context.JobCategories.AddRangeAsync(jobCategories);

            await SetIdentityInsertAndSaveChanges("JobCategories");
        }

        //async Task SeedUserJobPosts()
        //{
        //    if (await _context.UserJobPosts.AnyAsync()) return;

        //    var data = await File.ReadAllTextAsync("Seed/UserJobPostSeed.json");
        //    var userJobPosts = JsonSerializer.Deserialize<List<UserJobPost>>(data);

        //    await _context.UserJobPosts.AddRangeAsync(userJobPosts);

        //    await SetIdentityInsertAndSaveChanges("UserJobPosts");
        //}

        async Task SeedRoles()
        {
            if (await _roleManager.Roles.AnyAsync()) return;

            foreach (var role in Enum.GetNames<RoleType>())
            {
                await _roleManager.CreateAsync(new Role { Name = role });
            }
        }

        async Task SeedUsers()
        {
            if (await _userManager.Users.AnyAsync()) return;

            var data = await File.ReadAllTextAsync("Seed/UserSeed.json");
            var users = JsonSerializer.Deserialize<List<User>>(data);

            if (users == null) return;

            foreach (var user in users)
            {
                user.UserName = user.UserName.ToLower();
                var password = _config["AdminPassword"];
                await _userManager.CreateAsync(user, password);
                if(user.IsCompany)
                    await _userManager.AddToRoleAsync(user, RoleType.Company.ToString());
                else
                    await _userManager.AddToRoleAsync(user, RoleType.User.ToString());
            }
        }

        async Task SeedCountriesCitiesEtc()
        {
            await SeedCountries();
            await SeedCities();
        }

        async Task SeedCountries()
        {
            if (await _context.Countries.AnyAsync()) return;

            var data = await File.ReadAllTextAsync("Seed/CountrySeed.json");
            var countries = JsonSerializer.Deserialize<List<Country>>(data);
            
            await _context.Countries.AddRangeAsync(countries);

            await SetIdentityInsertAndSaveChanges("Countries");
        }

        async Task SeedCities()
        {
            if (await _context.Cities.AnyAsync()) return;

            var data = await File.ReadAllTextAsync("Seed/CitySeed.json");
            var cities = JsonSerializer.Deserialize<List<City>>(data);

            await _context.Cities.AddRangeAsync(cities);

            await SetIdentityInsertAndSaveChanges("Cities");
        }

        private async Task SetIdentityInsertAndSaveChanges(string tableName)
        {
            _context.Database.OpenConnection();
            try
            {
                _context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT dbo.{tableName} ON");
                await _context.SaveChangesAsync();
                _context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT dbo.{tableName} ON");
            }
            finally
            {
                _context.Database.CloseConnection();
            }
        }
    }
}
