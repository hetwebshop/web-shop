using API.Data;
using API.Data.ICompanyJobPostRepository;
using API.Data.Pagination;
using API.DTOs;
using API.Entities;
using API.Entities.CompanyJobPost;
using API.Entities.Notification;
using API.Helpers;
using API.Mappers;
using API.PaginationEntities;
using AutoMapper;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services.CompanyJobPostServices
{
    public class CompanyJobPostService : ICompanyJobPostService
    {
        private readonly ICompanyJobPostRepository companyJobPostRepository;
        private readonly DataContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly string UIBaseUrl;
        private readonly ISendNotificationsQueueClient _sendNotificationsQueueClient;

        public CompanyJobPostService(ICompanyJobPostRepository companyJobPostRepository, DataContext dbContext, IConfiguration configuration, IEmailService emailService, ISendNotificationsQueueClient sendNotificationsQueueClient)
        {
            this.companyJobPostRepository = companyJobPostRepository;
            _dbContext = dbContext;
            _configuration = configuration;
            _emailService = emailService;
            UIBaseUrl = configuration.GetSection("UIBaseUrl").Value;
            _sendNotificationsQueueClient = sendNotificationsQueueClient;
        }

        public async Task<PagedList<CompanyJobPostDto>> GetJobPostsAsync(AdsPaginationParameters adsParameters)
        {
            var companyJobPosts = await companyJobPostRepository.GetJobPostsAsync(adsParameters);
            return companyJobPosts.ToDto();
        }

        public async Task<PagedList<CompanyPublicInfoDto>> GetRegisteredCompaniesAsync(AdsPaginationParameters adsParameters)
        {
            var registeredCompanies = await companyJobPostRepository.GetRegisteredCompaniesAsync(adsParameters);
            return registeredCompanies.ToDto();
        }

        public async Task<PagedList<CompanyJobPostDto>> GetCompanyJobPostsAsync(AdsPaginationParameters adsParameters)
        {
            var companyJobPosts = await companyJobPostRepository.GetCompanyJobPostsAsync(adsParameters);
            return companyJobPosts.ToDto();
        }

        public async Task<CompanyJobPostDto> GetCompanyJobPostByIdAsync(int id)
        {
            var companyJobPost = await companyJobPostRepository.GetCompanyJobPostByIdAsync(id);
            return companyJobPost.ToDto();
        }

        public async Task<CompanyJobPostDto> CreateCompanyJobPostAsync(CompanyJobPostDto companyJobPostDto)
        {
            var newItem = await companyJobPostRepository.CreateCompanyJobPostAsync(companyJobPostDto.ToEntity());

            //var usersWithSimilarInterest = _dbContext.Users.Where(r => r.JobCategoryId == companyJobPostDto.JobCategoryId).ToList();
            //if (usersWithSimilarInterest.Any())
            //{
            //    var currentlyAddedJobPost = await companyJobPostRepository.GetCompanyJobPostByIdAsync(newItem.Id);
            //    var usersWithSimilarInterestIds = usersWithSimilarInterest.Select(r => r.Id);
            //    var usersNotifSettings = _dbContext.UserNotificationSettings.Where(r => usersWithSimilarInterestIds.Contains(r.UserId)).ToList();
            //    var emailTask = Task.Run(() => SendEmailsAsync(usersNotifSettings, currentlyAddedJobPost));

            //    foreach (var userNotif in usersNotifSettings)
            //    {
            //        if (userNotif.NotificationType == Entities.UserNotificationType.NewInterestingCompanyAdInApp && userNotif.IsEnabled)
            //        {
            //            var notification = new Notification()
            //            {
            //                UserId = userNotif.UserId.ToString(),
            //                CreatedAt = DateTime.UtcNow,
            //                IsRead = false,
            //                Link = UIBaseUrl + "company-ad-details/" + currentlyAddedJobPost.Id,
            //                Message = "Kreiran je novi oglas za posao koji bi vam mogao biti interesantan"
            //            };
            //            _dbContext.Notifications.Add(notification);
            //        }
            //    }
            //    await _dbContext.SaveChangesAsync();
            //}
            var jobPostNotificationMessage = new JobPostNotificationQueueMessage
            {
                JobPostId = newItem.Id,
            };
            await _sendNotificationsQueueClient.SendMessageToUserAsync(jobPostNotificationMessage);

            return newItem.ToDto();
        }

        private async Task SendEmailsAsync(List<UserNotificationSettings> userNotifSettings, CompanyJobPost newItem)
        {
            var userIdsToNotify = userNotifSettings
                .Where(setting => setting.NotificationType == UserNotificationType.NewInterestingCompanyAdEmail && setting.IsEnabled)
                .Select(setting => setting.UserId)
                .ToList();

            var usersToNotify = _dbContext.Users.Where(user => userIdsToNotify.Contains(user.Id)).ToList();

            var tasks = usersToNotify.Select(async user =>
            {
                if (user != null)
                {
                    string adUrl = _configuration.GetSection("UIBaseUrl").Value + $"company-ad-details/{newItem.Id}";
                    string messageBody = $@"
                    <p style='color: #66023C;'>Dragi <strong>{user.Email}</strong>,</p>
                    <p style='color: #66023C;'>Kreiran je novi oglas za posao u kategoriji: <strong>{newItem.JobCategory?.Name}</strong>.</p>
                    <p style='color: #66023C;'>Pogledajte detalje i prijavite se za ovu priliku što je prije moguće.</p>
                    <p style='text-align: center;'>
                        <a href='{adUrl}' style='display: inline-block; padding: 10px 20px; background-color: #66023C; color: #ffffff; text-decoration: none; border-radius: 5px;'>Pogledajte Oglas</a>
                    </p>";
                    var subject = "Novi oglas koji bi vas mogao zanimati";

                    var emailTemplate = EmailTemplateHelper.GenerateEmailTemplate(subject, messageBody, _configuration);
                    await _emailService.SendEmailWithTemplateAsync(user.Email, subject, emailTemplate);
                }
            }).ToList();

            await Task.WhenAll(tasks);
        }

        public async Task<CompanyJobPostDto> UpdateCompanyJobPostAsync(CompanyJobPostDto companyJobPostDto)
        {
            try
            {
                var newItem = await companyJobPostRepository.UpdateCompanyJobPostAsync(companyJobPostDto.ToEntity());
                return newItem.ToDto();
            }
            catch (AutoMapperMappingException ex)
            {
                throw;
            }
        }

        public async Task<CompanyJobPostDto> UpdateCompensationAndWorkEnvAsync(CompanyJobPostDto companyJobPostDto)
        {
            try
            {
                var newItem = await companyJobPostRepository.UpdateCompensationAndWorkEnvAsync(companyJobPostDto.ToEntity());
                return newItem.ToDto();
            }
            catch (AutoMapperMappingException ex)
            {
                throw;
            }
        }

        public async Task<bool> UpdateCompanyJobPostLogoAsync(int id, string logoUrl)
        {
            try
            {
                var isUpdated = await companyJobPostRepository.UpdateCompanyJobPostLogoAsync(id, logoUrl);
                return isUpdated;
            }
            catch (AutoMapperMappingException ex)
            {
                throw;
            }
        }

        public async Task<CompanyJobPostDto> UpdateQualificationsAndExpereinceAsync(CompanyJobPostDto companyJobPostDto)
        {
            try
            {
                var newItem = await companyJobPostRepository.UpdateQualificationsAndExperienceAsync(companyJobPostDto.ToEntity());
                return newItem.ToDto();
            }
            catch (AutoMapperMappingException ex)
            {
                throw;
            }
        }

        public async Task<CompanyJobPostDto> UpdateHowToApplyAsync(CompanyJobPostDto companyJobPostDto)
        {
            try
            {
                var newItem = await companyJobPostRepository.UpdateHowToApplyAsync(companyJobPostDto.ToEntity());
                return newItem.ToDto();
            }
            catch (AutoMapperMappingException ex)
            {
                throw;
            }
        }

        public async Task<List<CompanyJobPostDto>> GetCompanyAdsAsync(int companyId)
        {
            var companyAds = await companyJobPostRepository.GetCompanyAdsAsync(companyId);
            return companyAds.ToDto();
        }

        public async Task<bool> DeleteCompanyJobPostByIdAsync(int companyId, int jobPostId)
        {
            var jobPost = await companyJobPostRepository.GetCompanyJobPostByIdAsync(jobPostId);
            if (jobPost.User.CompanyId != companyId)
                return false;
            var deleted = await companyJobPostRepository.DeleteCompanyJobPostByIdAsync(jobPostId);
            return deleted;
        }

        public async Task<bool> CloseCompanyJobPostByIdAsync(int companyId, int jobPostId)
        {
            var jobPost = await companyJobPostRepository.GetCompanyJobPostByIdAsync(jobPostId);
            if (jobPost.User.CompanyId != companyId)
                return false;
            var closed = await companyJobPostRepository.CloseCompanyJobPostByIdAsync(jobPostId);
            return closed;
        }

        public async Task<bool> ReactivateCompanyJobPostByIdAsync(int companyId, int jobPostId)
        {
            var jobPost = await companyJobPostRepository.GetCompanyJobPostByIdAsync(jobPostId);
            if (jobPost.User.CompanyId != companyId)
                return false;
            var reactivated = await companyJobPostRepository.ReactivateCompanyJobPostByIdAsync(jobPostId);
            return reactivated;
        }
    }
}
