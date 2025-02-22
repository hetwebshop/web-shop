using API.Data;
using API.Data.ICompanyJobPostRepository;
using API.Data.IUserOfferRepository;
using API.Data.Pagination;
using API.DTOs;
using API.Entities;
using API.Entities.Applications;
using API.Entities.JobPost;
using API.Entities.Notification;
using API.Helpers;
using API.Mappers;
using API.PaginationEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        private string predictionApiUrl;
        private readonly IBlobStorageService blobStorageService;
        private readonly DataContext _dbContext;
        private readonly IEmailService emailService;
        private readonly string UIBaseUrl;
        private readonly IConfiguration _configuration;
        private readonly ISendNotificationsQueueClient _sendNotificationsQueueClient;

        public UserJobPostService(IUserJobPostRepository userJobPostRepository, IUnitOfWork uow, ICompanyJobPostRepository companyJobPostRepository, IConfiguration configuration, IBlobStorageService blobStorageService, DataContext dataContext, IEmailService emailService, ISendNotificationsQueueClient sendNotificationsQueueClient)
        {
            this.userJobPostRepository = userJobPostRepository;
            _uow = uow;
            companyJobPostRepo = companyJobPostRepository;
            predictionApiUrl = configuration.GetSection("PredictionApiUrl").Value;
            this.blobStorageService = blobStorageService;
            _dbContext = dataContext;
            this.emailService = emailService;
            UIBaseUrl = configuration.GetSection("UIBaseUrl").Value;
            _configuration = configuration;
            _sendNotificationsQueueClient = sendNotificationsQueueClient;
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

        public async Task<UserJobPostDto> CreateUserJobPostAsync(UserJobPostDto userJobPostDto)
        {
            try
            {
                var entity = userJobPostDto.ToEntity();
                var newItem = await userJobPostRepository.CreateUserJobPostAsync(entity);
                if (newItem == null)
                    throw new Exception("Pretplata koju ste postavili ne postoji");
                var dto = newItem.ToDto();
                var user = await _uow.UserRepository.GetUserByIdAsync(dto.SubmittingUserId);
                dto.CurrentUserCredits = user.Credits;

                //var companiesToNotify = _dbContext.CompanyJobCategoryInterests.Where(r => r.JobCategoryId == newItem.JobCategoryId).Select(r => r.UserId).ToList();
                //var companiesNotifSettings = _dbContext.CompanyNotificationPreferences.Where(r => companiesToNotify.Contains(r.UserId)).ToList();
                //var emailTask = Task.Run(() => SendEmailsAsync(companiesNotifSettings, newItem));
                //foreach (var companiesSetting in companiesNotifSettings)
                //{
                //    if(companiesSetting.NotificationType == Entities.Notification.CompanyNotificationType.newInterestingUserAdInApp && companiesSetting.IsEnabled)
                //    {
                //        var notification = new Notification()
                //        {
                //            UserId = companiesSetting.UserId.ToString(),
                //            CreatedAt = DateTime.UtcNow,
                //            IsRead = false,
                //            Link = UIBaseUrl + "ad-details/" + newItem.Id,
                //            Message = "Kreiran je novi oglas za posao koji bi vam mogao biti interesantan"
                //        };
                //        _dbContext.Notifications.Add(notification);
                //    }
                //}
                //await _dbContext.SaveChangesAsync();
                var jobPostNotificationMessage = new JobPostNotificationQueueMessage
                {
                    JobPostId = newItem.Id,
                };
                await _sendNotificationsQueueClient.SendMessageToCompanyAsync(jobPostNotificationMessage);

                return dto;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        private async Task SendEmailsAsync(List<CompanyNotificationPreferences> companiesNotifSettings, UserJobPost newItem)
        {
            var userIdsToNotify = companiesNotifSettings
                .Where(setting => setting.NotificationType == Entities.Notification.CompanyNotificationType.newInsterestingUserAdEmail && setting.IsEnabled)
                .Select(setting => setting.UserId)
                .ToList();

            var usersToNotify = _dbContext.Users.Where(user => userIdsToNotify.Contains(user.Id)).ToList();

            var tasks = usersToNotify.Select(async user =>
            {
                if (user != null)
                {
                    string adUrl = _configuration.GetSection("UIBaseUrl").Value + $"ad-details/{newItem.Id}";
                    string messageBody = $@"
                    <p style='color: #66023C;'>Dragi <strong>{user.Email}</strong>,</p>
                    <p style='color: #66023C;'>Kreiran je novi oglas za posao u kategoriji: <strong>{newItem.JobCategory?.Name}</strong>.</p>
                    <p style='color: #66023C;'>Pogledajte detalje i prijavite se za ovu priliku što je prije moguće.</p>
                    <p style='text-align: center;'>
                        <a href='{adUrl}' style='display: inline-block; padding: 10px 20px; background-color: #66023C; color: #ffffff; text-decoration: none; border-radius: 5px;'>Pogledajte Oglas</a>
                    </p>";
                    var subject = "Novi oglas koji bi vas mogao zanimati";

                    var emailTemplate = EmailTemplateHelper.GenerateEmailTemplate(subject, messageBody, _configuration);
                    await emailService.SendEmailWithTemplateAsync(user.Email, subject, emailTemplate);
                }
            }).ToList();

            await Task.WhenAll(tasks);
        }

        private async Task SendNewApplicantEmailsAsync(List<CompanyNotificationPreferences> companiesNotifSettings, string position, int userApplicationId, string emailForReceivingApplications, int userId)
        {
            var userIdsToNotify = companiesNotifSettings
            .Where(setting => setting.NotificationType == Entities.Notification.CompanyNotificationType.newApplicantEmail && setting.IsEnabled)
            .Select(setting => setting.UserId)
            .ToList();

            var user = await _dbContext.Users
                .Where(r => r.Id == userId)
                .Include(r => r.Company) 
                .FirstOrDefaultAsync();   

            if (user != null && user.Company != null && userIdsToNotify.Contains(userId)) 
            {
                var applicationUrl = UIBaseUrl + $"company-settings/candidate-details/{userApplicationId}";

                string messageBody = $@"
            <p style='color: #66023C;'>Dragi <strong>{user.Company.CompanyName}</strong>,</p>
            <p style='color: #66023C;'>Dobili ste novu prijavu za poziciju: <strong>{position}</strong>.</p>
            <p style='color: #66023C;'>Pogledajte prijavu i obavite daljnje radnje prema vašim potrebama.</p>
            <p style='text-align: center;'>
                <a href='{applicationUrl}' style='display: inline-block; padding: 10px 20px; background-color: #66023C; color: #ffffff; text-decoration: none; border-radius: 5px;'>Pogledajte Prijavu</a>
            </p>";

                var subject = "Nova prijava na vaš oglas za posao";
                var emailTemplate = EmailTemplateHelper.GenerateEmailTemplate(subject, messageBody, _configuration);

                try
                {
                    await emailService.SendEmailWithTemplateAsync(emailForReceivingApplications, subject, emailTemplate);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public async Task<UserApplicationDto> CreateUserApplicationAsync(UserApplicationDto userApplication)
        {
            try
            {
                var companyJobPost = await companyJobPostRepo.GetCompanyJobPostByIdAsync(userApplication.CompanyJobPostId);
                if (companyJobPost == null)
                    return null;
                PredictionApiResponse predictionApiResponse = null;

                var entity = userApplication.ToEntity();
                var newItem = await userJobPostRepository.CreateUserApplicationAsync(entity);
                if (companyJobPost.PricingPlan.Name == "Premium")
                {
                    //using (var client = new HttpClient())
                    //using (var content = new MultipartFormDataContent())
                    //{
                    //    content.Add(new StringContent(companyJobPost.Position), "job_description");

                   var file = await blobStorageService.GetFileAsync(userApplication.CvFileName);
                    //    var fileContent = new ByteArrayContent(file.FileContent);
                    //    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
                    //    content.Add(fileContent, "cv", Path.GetFileName(userApplication.CvFilePath));

                    //    HttpResponseMessage response = await client.PostAsync(predictionApiUrl, content);

                    //    if (response.IsSuccessStatusCode)
                    //    {
                    //        var responseStr = await response.Content.ReadAsStringAsync();
                    //        var jsonDataStartIndex = responseStr.IndexOf('{');
                    //        if (jsonDataStartIndex >= 0)
                    //        {
                    //            var validJsonStr = responseStr.Substring(jsonDataStartIndex);
                    //            predictionApiResponse = JsonConvert.DeserializeObject<PredictionApiResponse>(validJsonStr);
                    //        }
                    //    }
                    //}
                    var applicantPredictionMessage = new NewApplicantPredictionQueueMessage()
                    {
                        CompanyJobPostId = companyJobPost.Id,
                        UserApplicationId = newItem.Id,
                        //Position = companyJobPost.Position,
                        //MotivationLetter = userApplication.CoverLetter,
                        //CvFileUrl = userApplication.CvFileName,
                        //YearsOfExperience = userApplication.YearsOfExperience,
                        //UserPreviousCompanies = userApplication.PreviousCompanies.Select(r => new UserApplicationPreviousCompanies
                        //{
                        //    CompanyName = r.CompanyName,  
                        //    Position = r.Position,     
                        //    StartYear = r.StartYear,    
                        //    EndYear = r.EndYear,
                        //    Description = r.Description
                        //}).ToList(),
                        //UserEducations = userApplication.Educations.Select(r => new UserApplicationEducation
                        //{
                        //    Degree = r.Degree,
                        //    EducationEndYear = r.EducationEndYear,
                        //    EducationStartYear = r.EducationStartYear,
                        //    FieldOfStudy = r.FieldOfStudy,
                        //    InstitutionName = r.InstitutionName,
                        //    University = r.University,
                            
                        //}).ToList()
                    };
                    await _sendNotificationsQueueClient.SendNewApplicantPredictionMessageAsync(applicantPredictionMessage);
                }

                //if(predictionApiResponse != null)
                //{
                //    entity.AIMatchingDescription = predictionApiResponse.Obrazlozenje;
                //    entity.AIMatchingEducationLevel = predictionApiResponse.Opis.PoklapanjeDomeneZnanja;
                //    entity.AIMatchingExperience = predictionApiResponse.Opis.PoklapanjeIskustva;
                //    entity.AIMatchingSkills = predictionApiResponse.Opis.PoklapanjeVjestina;
                //    entity.AIMatchingResult = predictionApiResponse.Rezultat;
                //}


                //var companyNotifPreferences = _dbContext.CompanyNotificationPreferences.Where(r => r.UserId == companyJobPost.SubmittingUserId && r.IsEnabled == true && (r.NotificationType == CompanyNotificationType.newApplicantInApp || r.NotificationType == CompanyNotificationType.newApplicantEmail))?.ToList();

                //if (companyNotifPreferences != null && companyNotifPreferences.Any())
                //{
                //    var emailTask = Task.Run(() => SendNewApplicantEmailsAsync(companyNotifPreferences, companyJobPost.Position, newItem.Id, companyJobPost.EmailForReceivingApplications, companyJobPost.SubmittingUserId));
                //    var inAppSetting = companyNotifPreferences.FirstOrDefault(r => r.NotificationType == CompanyNotificationType.newApplicantInApp);
                //    if(inAppSetting != null)
                //    {
                //        var notification = new Notification()
                //        {
                //            UserId = companyJobPost.SubmittingUserId.ToString(),
                //            CreatedAt = DateTime.UtcNow,
                //            IsRead = false,
                //            Link = UIBaseUrl + "company-settings/job-candidates/" + newItem.Id,
                //            Message = "Kreiran je nova aplikacija za posao na vašem oglasu " + companyJobPost.Position
                //        };
                //        _dbContext.Notifications.Add(notification);
                //        await _dbContext.SaveChangesAsync();
                //    }
                //}

                var newApplicantMessage = new NewApplicantQueueMessage()
                {
                    JobPostId = companyJobPost.Id,
                    UserApplicationId = newItem.Id
                };
                await _sendNotificationsQueueClient.SendNewApplicantMessageToCompanyAsync(newApplicantMessage);
                var dto = newItem.ToDto();
                return dto;
            }
            catch (Exception ex)
            {
                throw;
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
