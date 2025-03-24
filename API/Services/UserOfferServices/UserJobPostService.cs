using API.Data;
using API.Data.ICompanyJobPostRepository;
using API.Data.IUserOfferRepository;
using API.Data.Pagination;
using API.DTOs;
using API.Entities;
using API.Entities.Applications;
using API.Entities.JobPost;
using API.Entities.Notification;
using API.Entities.Payment;
using API.Helpers;
using API.Mappers;
using API.PaginationEntities;
using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace API.Services.UserOfferServices
{
    public class UserJobPostService : IUserJobPostService
    {
        private readonly IUserJobPostRepository userJobPostRepository;
        private readonly IUnitOfWork _uow;
        private readonly ICompanyJobPostRepository companyJobPostRepo;
        private readonly IBlobStorageService blobStorageService;
        private readonly DataContext _dbContext;
        private readonly IEmailService emailService;
        private readonly string UIBaseUrl;
        private readonly IConfiguration _configuration;
        private readonly ISendNotificationsQueueClient _sendNotificationsQueueClient;
        private readonly ILogger<UserJobPostService> _logger;

        public UserJobPostService(IUserJobPostRepository userJobPostRepository, IUnitOfWork uow, ICompanyJobPostRepository companyJobPostRepository, IConfiguration configuration, IBlobStorageService blobStorageService, DataContext dataContext, IEmailService emailService, ISendNotificationsQueueClient sendNotificationsQueueClient, ILogger<UserJobPostService> logger)
        {
            this.userJobPostRepository = userJobPostRepository;
            _uow = uow;
            companyJobPostRepo = companyJobPostRepository;
            this.blobStorageService = blobStorageService;
            _dbContext = dataContext;
            this.emailService = emailService;
            UIBaseUrl = configuration.GetSection("UIBaseUrl").Value;
            _configuration = configuration;
            _sendNotificationsQueueClient = sendNotificationsQueueClient;
            _logger = logger;
        }

        public async Task<PagedList<UserJobPostDto>> GetJobPostsAsync(AdsPaginationParameters adsParameters)
        {
            var userJobPosts = await userJobPostRepository.GetJobPostsAsync(adsParameters);
            return userJobPosts.ToDto();
        }

        public async Task<PagedList<UserJobPostDto>> GetUserJobPostsAsync(AdsPaginationParameters adsParameters)
        {
            var userJobPosts = await userJobPostRepository.GetUserJobPostsAsync(adsParameters);
            return userJobPosts.ToDto();
        }

        public async Task<UserJobPostDto> GetUserJobPostByIdAsync(int id)
        {
            var userJobPost = await userJobPostRepository.GetUserJobPostByIdAsync(id);
            return userJobPost.ToDto();
        }

        public async Task<UserJobPostDto> CreateUserJobPostAsync(UserJobPostDto userJobPostDto, User user)
        {
            try
            {
                var entity = userJobPostDto.ToEntity();
                bool isUserProfileCvFileSubmitted = userJobPostDto.IsUserProfileCvFileSubmitted ?? false;
                if (userJobPostDto.CvFile != null)
                {
                    var fileUrl = await blobStorageService.UploadFileAsync(userJobPostDto.CvFile);
                    var decodedFileUrl = Uri.UnescapeDataString(fileUrl);
                    userJobPostDto.CvFilePath = decodedFileUrl;
                    userJobPostDto.CvFileName = userJobPostDto.CvFile.FileName;
                }
                else if (isUserProfileCvFileSubmitted == true)
                {
                    userJobPostDto.CvFilePath = user.CvFilePath;
                }
                var newItem = await userJobPostRepository.CreateUserJobPostAsync(entity);
                var dto = newItem.ToDto();
                var jobPostNotificationMessage = new NotificationEventMessage
                {
                    JobPostId = newItem.Id,
                    NotificationType = NotificationType.SendCompanyNotificationOnCreateUserAd
                };
                await _sendNotificationsQueueClient.SendMessageToCompanyAsync(jobPostNotificationMessage);
                _logger.LogInformation($"Notification sending finished.");

                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ERROR] Job post creation failed for UserId: {userJobPostDto.SubmittingUserId}. Reason: {ex.Message}");
                throw; //Re-throw the exception so the controller can handle it
            }
        }

        public async Task<UserApplicationDto> CreateUserApplicationAsync(UserApplicationDto userApplication, User submittingUser)
        {
            try
            {
                _logger.LogInformation($"[START] Creating application for UserId: {submittingUser.Id}, JobPostId: {userApplication.CompanyJobPostId}");
                var companyJobPost = await companyJobPostRepo.GetCompanyJobPostByIdAsync(userApplication.CompanyJobPostId);
                if (companyJobPost == null)
                {
                    _logger.LogWarning($"[FAILED] JobPostId: {userApplication.CompanyJobPostId} does not exist. UserId: {submittingUser.Id}");
                    throw new Exception("Oglas za posao ne postoji.");
                }
                try
                {
                    if (userApplication.CvFile != null)
                    {
                        var fileUrl = await blobStorageService.UploadFileAsync(userApplication.CvFile);
                        userApplication.CvFilePath = Uri.UnescapeDataString(fileUrl);
                        userApplication.CvFileName = userApplication.CvFile.FileName;
                    }
                    else if (userApplication.IsUserProfileCvFileSubmitted == true)
                    {
                        userApplication.CvFilePath = submittingUser.CvFilePath;
                        userApplication.CvFileName = submittingUser.CvFileName;
                    }
                }
                catch (Exception fileEx)
                {
                    _logger.LogError($"[ERROR] CV upload failed for UserId: {submittingUser.Id}. Reason: {fileEx.Message}");
                    throw new Exception("Došlo je do greške prilikom učitavanja CV-a. Molimo pokušajte ponovo.");
                }
                _logger.LogInformation($"[SUCCESS] CV uploaded for UserId: {submittingUser.Id}, JobPostId: {userApplication.CompanyJobPostId}");

                var entity = userApplication.ToEntity();
                var newItem = await userJobPostRepository.CreateUserApplicationAsync(entity);
                
                _logger.LogInformation($"[SUCCESS] Application created successfully for UserId: {submittingUser.Id}, JobPostId: {companyJobPost.Id}, ApplicationId: {newItem.Id}");

                if (companyJobPost.PricingPlan.Name == "Premium")
                {
                    var applicantPredictionMessage = new NewApplicantPredictionQueueMessage()
                    {
                        CompanyJobPostId = companyJobPost.Id,
                        UserApplicationIds = new List<int> { newItem.Id },
                    };
                    try
                    {
                        await _sendNotificationsQueueClient.SendNewApplicantPredictionMessageAsync(applicantPredictionMessage);
                        _logger.LogInformation($"[SUCCESS] AI Prediction Message Sent for JobPostId: {companyJobPost.Id}, ApplicationId: {newItem.Id}");
                    }
                    catch (Exception predictionEx)
                    {
                        _logger.LogError($"[ERROR] Failed to send AI Prediction message for JobPostId: {companyJobPost.Id}, ApplicationId: {newItem.Id}. Reason: {predictionEx.Message}");
                    }
                }
                var newApplicantMessage = new NotificationEventMessage()
                {
                    JobPostId = companyJobPost.Id,
                    UserApplicationIds = new List<int> { newItem.Id },
                    NotificationType = NotificationType.SendCompanyNotificationOnNewUserApplication
                };

                await _sendNotificationsQueueClient.SendNewApplicantMessageToCompanyAsync(newApplicantMessage);
                _logger.LogInformation($"Notification sending finished.");

                var dto = newItem.ToDto();
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ERROR] Application creation failed for UserId: {submittingUser.Id}, JobPostId: {userApplication.CompanyJobPostId}. Reason: {ex.Message}");
                throw new Exception("Došlo je do greške prilikom kreiranja prijave. Pokušajte ponovo kasnije.");
            }
        }


        public async Task<UserJobPostDto> UpdateUserJobPostAsync(UserJobPostDto userJobPostDto)
        {
            var newItem = await userJobPostRepository.UpdateUserJobPostAsync(userJobPostDto.ToEntity());
            return newItem.ToDto();
        }

        public async Task<List<JobCategory>> GetAllJobCategoriesAsync()
        {
            var jobCategories = await userJobPostRepository.GetAllJobCategoriesAsync();
            return jobCategories;
        }

        public async Task<List<JobType>> GetAllJobTypesAsync()
        {
            var jobTypes = await userJobPostRepository.GetAllJobTypesAsync();
            return jobTypes;
        }

        public async Task<List<AdvertisementType>> GetAllAdvertisementTypesAsync()
        {
            var adTypes = await userJobPostRepository.GetAllAdvertisementTypesAsync();
            return adTypes;
        }

        public async Task<List<UserJobPostDto>> GetMyAdsAsync(int userId)
        {
            var myAds = await userJobPostRepository.GetMyAdsAsync(userId);
            return myAds.ToDto();
        }

        public async Task<bool> DeleteUserJobPostByIdAsync(int userId, int jobPostId)
        {
            var jobPost = await userJobPostRepository.GetUserJobPostByIdAsync(jobPostId);
            if (jobPost.SubmittingUserId != userId)
                return false;
            var deleted = await userJobPostRepository.DeleteUserJobPostByIdAsync(jobPostId);
            return deleted;
        }

        public async Task<bool> CloseUserJobPostByIdAsync(int userId, int jobPostId)
        {
            var jobPost = await userJobPostRepository.GetUserJobPostByIdAsync(jobPostId);
            if (jobPost.SubmittingUserId != userId)
                return false;
            var closed = await userJobPostRepository.CloseUserJobPostByIdAsync(jobPostId);
            return closed;
        }

        public async Task<bool> ReactivateUserJobPostByIdAsync(int userId, int jobPostId)
        {
            var jobPost = await userJobPostRepository.GetUserJobPostByIdAsync(jobPostId);
            if (jobPost.SubmittingUserId != userId)
                return false;
            var reactivated = await userJobPostRepository.ReactivateUserJobPostByIdAsync(jobPostId);
            return reactivated;
        }

        public async Task<UserJobPostDto> UpdateAdUserBaseInfo(UserAdBaseInfoRequest userAdBaseInfoRequest)
        {
            var updated = await userJobPostRepository.UpdateAdUserBaseInfo(userAdBaseInfoRequest);
            return updated.ToDto();
        }
        public async Task<UserJobPostDto> UpdateAdInfo(UserAdInfoUpdateRequest userAdInfoUpdateRequest)
        {
            var updated = await userJobPostRepository.UpdateAdInfo(userAdInfoUpdateRequest);
            var dto = updated.ToDto();
            return dto;
        }
    }

    public class PredictionApiResponse
    {
        public int Rezultat { get; set; }
        [JsonProperty("opis")]
        public Opis Opis { get; set; }
        [JsonProperty("Obrazloženje")]
        public string Obrazlozenje { get; set; }
    }

    public class Opis
    {
        [JsonProperty("Poklapanje Vještina")]
        public int PoklapanjeVjestina { get; set; }
        [JsonProperty("Poklapanje Iskustva")]
        public int PoklapanjeIskustva { get; set; }
        [JsonProperty("Poklapanje Domene Znanja")]
        public int PoklapanjeDomeneZnanja { get; set; }
    }
}
