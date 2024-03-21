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
            await SeedUsers();

            //JobPost
            await SeedUserJobPostModels();
        }

        async Task SeedUserJobPostModels()
        {
            await SeedJobTypes();
            await SeedJobPostStatuses();
            await SeedJobCategoriesAndSubcategories();
            await SeedUserJobPosts();
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

        async Task SeedUserJobPosts()
        {
            if (await _context.UserJobPosts.AnyAsync()) return;

            var data = await File.ReadAllTextAsync("Seed/UserJobPostSeed.json");
            var userJobPosts = JsonSerializer.Deserialize<List<UserJobPost>>(data);

            await _context.UserJobPosts.AddRangeAsync(userJobPosts);

            await SetIdentityInsertAndSaveChanges("UserJobPosts");
        }

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
                user.Address = await GetRandomAddress();
                user.Address.StreetName = user.Name;

                switch (user.UserName)
                {
                    case Constants.Admin:
                        await _userManager.CreateAsync(user, _config["AdminPassword"]);
                        await _userManager.AddToRolesAsync(user, Enum.GetNames<RoleType>());
                        break;
                    case Constants.TestUser:
                        await _userManager.CreateAsync(user, _config["AdminPassword"]);
                        await _userManager.AddToRolesAsync(user, new[] { RoleType.User.ToString(), RoleType.TrackModerator.ToString(), RoleType.StoreModerator.ToString() });
                        break;
                    default:
                        await _userManager.CreateAsync(user, _config["AdminPassword"]);
                        await _userManager.AddToRoleAsync(user, RoleType.User.ToString());
                        break;
                }
            }
        }

        async Task SeedCountriesCitiesEtc()
        {
            await SeedCountries();
            await SeedCities();
            //await SeedMunicipalities();
            //await SeedCantons();
        }

        async Task SeedCountries()
        {
            if (await _context.Countries.AnyAsync()) return;

            var data = await File.ReadAllTextAsync("Seed/CountrySeed.json");
            var countries = JsonSerializer.Deserialize<List<Country>>(data);
            
            await _context.Countries.AddRangeAsync(countries);

            await SetIdentityInsertAndSaveChanges("Countries");
        }

        //async Task SeedCantons()
        //{
        //    if (await _context.Cantons.AnyAsync()) return;

        //    var data = await File.ReadAllTextAsync("Seed/CantonSeed.json");
        //    var cantons = JsonSerializer.Deserialize<List<Canton>>(data);

        //    await _context.Cantons.AddRangeAsync(cantons);

        //    await SetIdentityInsertAndSaveChanges("Cantons");
        //}

        async Task SeedCities()
        {
            if (await _context.Cities.AnyAsync()) return;

            var data = await File.ReadAllTextAsync("Seed/CitySeed.json");
            var cities = JsonSerializer.Deserialize<List<City>>(data);

            await _context.Cities.AddRangeAsync(cities);

            await SetIdentityInsertAndSaveChanges("Cities");
        }

        //async Task SeedMunicipalities()
        //{
        //    if (await _context.Municipalities.AnyAsync()) return;

        //    var data = await File.ReadAllTextAsync("Seed/MunicipalitySeed.json");
        //    var municipalities = JsonSerializer.Deserialize<List<Municipality>>(data);

        //    await _context.Municipalities.AddRangeAsync(municipalities);
        //    await _context.SaveChangesAsync();
        //}

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

        async Task<UserAddress> GetRandomAddress()
        {
            var cities = _context.Cities.ToList();
            var random = new Random();
            int randomIndex = random.Next(0, cities.Count);

            var faker = new Faker<UserAddress>()
                .RuleFor(a => a.StreetName, f => f.Address.StreetName())
                .RuleFor(a => a.StreetNumber, f => f.Address.BuildingNumber())
                .RuleFor(a => a.CityId, f => cities[randomIndex].Id);

            var randomAddress = faker.Generate();
            return randomAddress;
        }

    }
}
