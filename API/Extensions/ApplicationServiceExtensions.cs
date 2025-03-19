using System;
using API.Data;
using API.Entities;
using API.Helpers;
using API.Seed;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using API.Services.UserOfferServices;
using API.Data.IUserOfferRepository;
using API.Data.Pagination;
using API.Services.CompanyJobPostServices;
using API.Data.ICompanyJobPostRepository;
using API.Mappers;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<DataContext>(options =>
            {
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                string connStr = config.GetConnectionString("DefaultConnection");

                options.UseSqlServer(connStr);
            });

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddSingleton<ISendNotificationsQueueClient, SendNotificationsQueueClient>();
            //services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<SeedData, SeedData>();

            services.AddSingleton(typeof(ISortHelper<>), typeof(SortHelper<>));

            services.AddSingleton<IEmailService, AzureEmailService>();

            services.AddSingleton<IBlobStorageService, BlobStorageService>();
            
            //jobPosts
            services.AddScoped<IUserJobPostService, UserJobPostService>();
            services.AddScoped<IUserJobPostRepository, UserJobPostRepository>();
            services.AddScoped<ICompanyJobPostService, CompanyJobPostService>();
            services.AddScoped<ICompanyJobPostRepository, CompanyJobPostRepository>();
            

            //locations
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<ILocationService, LocationService>();

            services.AddScoped<IPricingPlanRepository, PricingPlanRepository>();
            services.AddScoped<IPricingPlanService, PricingPlanService>();

            services.AddScoped<IUserApplicationsRepository, UserApplicationsRepository>();

            services.AddHttpClient<RecaptchaService>();

            services.AddIdentity<User, Role>(opt => { 
                opt.Password.RequireNonAlphanumeric = true;
                opt.Password.RequiredLength = 8;
                opt.Password.RequireDigit = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireLowercase = true;
                opt.Tokens.EmailConfirmationTokenProvider = "Default";
                opt.SignIn.RequireConfirmedEmail = true;
                opt.Tokens.EmailConfirmationTokenProvider = "Default";
            })
            .AddEntityFrameworkStores<DataContext>()
            .AddDefaultTokenProviders();

            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromMinutes(15);
            });

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                //.AddCookie(x =>
                //{
                //    x.Cookie.Name = "accessToken";
                //})
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"])),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    };
                    options.SaveToken = true;

                    //options.Events = new JwtBearerEvents
                    //{
                    //    OnMessageReceived = ctx =>
                    //    {
                    //        ctx.Request.Cookies.TryGetValue("accessToken", out var accessToken);
                    //        if (!string.IsNullOrEmpty(accessToken))
                    //            ctx.Token = accessToken;
                    //        return Task.CompletedTask;
                    //    }
                    //};
                });

            return services;
        }
    }
}
