using System;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Dapper;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using jobify_azure_function;

namespace jobify_notification_function
{
    public class SendNotifications
    {
        private readonly ILogger<SendNotifications> _logger;
        private readonly string UIBaseUrl;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly IEmailService _emailService;

        public SendNotifications(ILogger<SendNotifications> logger, IConfiguration configuration, IEmailService emailService)
        {
            _logger = logger;
            _configuration = configuration;
            UIBaseUrl = configuration.GetSection("UIBaseUrl").Value;
            _connectionString = configuration.GetSection("DBConnectionString").Value;
            _emailService = emailService;
        }

        [Function("SendUserNotificationsOnCreateCompanyAd")]
        public async Task SendUserNotificationsOnCreateCompanyAd([QueueTrigger("jobify-user-notification-queue", Connection = "AzureWebJobsStorage")] QueueMessage message)
        {
            var jobPostNotificationMessage = JsonConvert.DeserializeObject<JobPostNotificationQueueMessage>(message.MessageText);

            _logger.LogInformation($"Processing notification for job post: {jobPostNotificationMessage.JobPostId}");

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Get the job post by ID using Dapper
                var query = "SELECT * FROM CompanyJobPosts WHERE Id = @JobPostId";
                var newItem = await connection.QueryFirstOrDefaultAsync<dynamic>(query, new { JobPostId = jobPostNotificationMessage.JobPostId });

                if (newItem != null)
                {
                    // Get users with similar interests
                    var usersWithSimilarInterestQuery = "SELECT * FROM AspNetUsers WHERE JobCategoryId = @JobCategoryId";
                    var usersWithSimilarInterest = await connection.QueryAsync<dynamic>(usersWithSimilarInterestQuery, new { JobCategoryId = newItem.JobCategoryId });

                    if (usersWithSimilarInterest.Any())
                    {
                        // Get user notification settings using Dapper
                        var userNotifSettingsQuery = "SELECT * FROM UserNotificationSettings WHERE UserId IN @UserIds";
                        var userNotifSettings = await connection.QueryAsync<dynamic>(userNotifSettingsQuery, new { UserIds = usersWithSimilarInterest.Select(u => u.Id).ToArray() });

                        // In-app notifications using Dapper
                        foreach (var userNotif in userNotifSettings.Where(r => r.NotificationType == 0 && r.IsEnabled))
                        {
                            var notification = new
                            {
                                UserId = userNotif.UserId.ToString(),
                                CreatedAt = DateTime.UtcNow,
                                IsRead = false,
                                Link = $"{UIBaseUrl}company-ad-details/{newItem.Id}",
                                Message = "Kreiran je novi oglas za posao koji bi vam mogao biti interesantan"
                            };

                            var insertNotificationQuery = "INSERT INTO Notifications (UserId, CreatedAt, IsRead, Link, Message) VALUES (@UserId, @CreatedAt, @IsRead, @Link, @Message)";
                            await connection.ExecuteAsync(insertNotificationQuery, notification);
                        }

                        // Email notifications using Dapper
                        var emailTasks = userNotifSettings
                            .Where(setting => setting.NotificationType == 1 && setting.IsEnabled)
                            .Select(async userNotif =>
                            {
                                var user = usersWithSimilarInterest.FirstOrDefault(u => u.Id == userNotif.UserId);
                                if (user != null)
                                {
                                    string adUrl = UIBaseUrl + $"company-ad-details/{newItem.Id}";
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

                        // Execute all email sending tasks asynchronously
                        await Task.WhenAll(emailTasks);
                    }
                }
            }
        }

        [Function("SendCompanyNotificationOnCreateUserAd")]
        public async Task SendCompanyNotificationOnCreateUserAd([QueueTrigger("jobify-company-notification-queue", Connection = "AzureWebJobsStorage")] QueueMessage message)
        {
            var jobPostNotificationMessage = JsonConvert.DeserializeObject<JobPostNotificationQueueMessage>(message.MessageText);

            _logger.LogInformation($"Processing notification for job post: {jobPostNotificationMessage.JobPostId}");

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Get the job post by ID
                var query = "SELECT * FROM UserJobPosts WHERE Id = @JobPostId";
                var newItem = await connection.QueryFirstOrDefaultAsync<dynamic>(query, new { JobPostId = jobPostNotificationMessage.JobPostId });

                if (newItem != null)
                {
                    // Get companies that are interested in the job category
                    var companiesToNotify = await connection.QueryAsync<dynamic>(
                        "SELECT UserId FROM CompanyJobCategoryInterests WHERE JobCategoryId = @JobCategoryId",
                        new { JobCategoryId = newItem.JobCategoryId }
                    );

                    var companiesNotifSettings = await connection.QueryAsync<dynamic>(
                        "SELECT * FROM CompanyNotificationPreferences WHERE UserId IN @UserIds",
                        new { UserIds = companiesToNotify.Select(c => c.UserId).ToArray() }
                    );

                    var category = await connection.QueryFirstOrDefaultAsync<string>(
                        "SELECT Name FROM JobCategories WHERE Id = @JobCategoryId",
                        new { JobCategoryId = newItem.JobCategoryId }
                    );

                    // In-app notifications
                    foreach (var companiesSetting in companiesNotifSettings.Where(r => r.NotificationType == 2 && r.IsEnabled))
                    {
                        var notification = new
                        {
                            UserId = companiesSetting.UserId.ToString(),
                            CreatedAt = DateTime.UtcNow,
                            IsRead = false,
                            Link = UIBaseUrl + "ad-details/" + newItem.Id,
                            Message = $"Kreiran je novi korisnički oglas za posao u kategoriji {category} koji bi vam mogao biti interesantan"
                        };

                        var insertNotificationQuery = "INSERT INTO Notifications (UserId, CreatedAt, IsRead, Link, Message) VALUES (@UserId, @CreatedAt, @IsRead, @Link, @Message)";
                        await connection.ExecuteAsync(insertNotificationQuery, notification);
                    }

                    // Send email notifications
                    var emailTasks = companiesNotifSettings
                        .Where(setting => setting.NotificationType == 3 && setting.IsEnabled)
                        .Select(async companiesSetting =>
                        {
                            var user = companiesToNotify.FirstOrDefault(u => u.UserId == companiesSetting.UserId);
                            if (user != null)
                            {
                                string adUrl = UIBaseUrl + $"ad-details/{newItem.Id}";
                                string messageBody = $@"
                            <p style='color: #66023C;'>Dragi <strong>{user.Email}</strong>,</p>
                            <p style='color: #66023C;'>Kreiran je novi korisnički oglas za posao u kategoriji: <strong>{category}</strong>.</p>
                            <p style='color: #66023C;'>Pogledajte detalje i kontaktirajte korisnika kako biste saznali više.</p>
                            <p style='text-align: center;'>
                                <a href='{adUrl}' style='display: inline-block; padding: 10px 20px; background-color: #66023C; color: #ffffff; text-decoration: none; border-radius: 5px;'>Pogledajte Oglas</a>
                            </p>";
                                var subject = "Novi oglas koji bi vas mogao zanimati";

                                var emailTemplate = EmailTemplateHelper.GenerateEmailTemplate(subject, messageBody, _configuration);
                                await _emailService.SendEmailWithTemplateAsync(user.Email, subject, emailTemplate);
                            }
                        }).ToList();

                    // Execute all email sending tasks asynchronously
                    await Task.WhenAll(emailTasks);
                }
            }
        }

        [Function("SendCompanyNotificationOnNewUserApplication")]
        public async Task Run([QueueTrigger("jobify-new-applicant-queue", Connection = "AzureWebJobsStorage")] QueueMessage message)
        {
            var userApplicationMessage = JsonConvert.DeserializeObject<NewApplicantQueueMessage>(message.MessageText);

            _logger.LogInformation($"Processing new applicant notification for job post: {userApplicationMessage.JobPostId}");

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Get the job post by ID
                var query = "SELECT * FROM CompanyJobPosts WHERE Id = @JobPostId";
                var companyJobPost = await connection.QueryFirstOrDefaultAsync<dynamic>(query, new { JobPostId = userApplicationMessage.JobPostId });

                if (companyJobPost != null)
                {
                    // Fetch company notification preferences
                    var preferencesQuery = "SELECT * FROM CompanyNotificationPreferences WHERE UserId = @UserId AND IsEnabled = 1 AND (NotificationType = @InAppType OR NotificationType = @EmailType)";
                    var companyNotifPreferences = await connection.QueryAsync<dynamic>(preferencesQuery, new
                    {
                        UserId = companyJobPost.SubmittingUserId,
                        InAppType = 0,
                        EmailType = 1
                    });

                    if (companyNotifPreferences != null && companyNotifPreferences.Any())
                    {
                        var emailNotifEnabled = companyNotifPreferences.FirstOrDefault(r => r.NotificationType == 1);
                        if(emailNotifEnabled != null)
                        {
                            var userEmail = companyJobPost.EmailForReceivingApplications;
                            var companyName = companyJobPost.CompanyName;
                            var position = companyJobPost.Position;
                            var applicationUrl = UIBaseUrl + $"company-settings/candidate-details/{userApplicationMessage.UserApplicationId}";

                            string messageBody = $@"
            <p style='color: #66023C;'>Dragi <strong>{companyName}</strong>,</p>
            <p style='color: #66023C;'>Dobili ste novu prijavu za poziciju: <strong>{position}</strong>.</p>
            <p style='color: #66023C;'>Pogledajte prijavu i obavite daljnje radnje prema vašim potrebama.</p>
            <p style='text-align: center;'>
                <a href='{applicationUrl}' style='display: inline-block; padding: 10px 20px; background-color: #66023C; color: #ffffff; text-decoration: none; border-radius: 5px;'>Pogledajte Prijavu</a>
            </p>";

                            var subject = "Nova prijava na vaš oglas za posao";
                            var emailTemplate = EmailTemplateHelper.GenerateEmailTemplate(subject, messageBody, _configuration);

                            try
                            {
                                await _emailService.SendEmailWithTemplateAsync(userEmail, subject, emailTemplate);
                            }
                            catch (Exception ex)
                            {
                                throw;
                            }
                        }

                        // In-app notifications
                        var inAppSetting = companyNotifPreferences.FirstOrDefault(r => r.NotificationType == 0);
                        if (inAppSetting != null)
                        {
                            var notification = new
                            {
                                UserId = companyJobPost.SubmittingUserId.ToString(),
                                CreatedAt = DateTime.UtcNow,
                                IsRead = false,
                                Link = $"{UIBaseUrl}company-settings/job-candidates/{userApplicationMessage.UserApplicationId}",
                                Message = $"Kreirana je nova aplikacija za posao na vašem oglasu {companyJobPost.Position}"
                            };

                            var insertNotificationQuery = "INSERT INTO Notifications (UserId, CreatedAt, IsRead, Link, Message) VALUES (@UserId, @CreatedAt, @IsRead, @Link, @Message)";
                            await connection.ExecuteAsync(insertNotificationQuery, notification);

                        }
                    }
                }
            }
        }
    }
    public class JobPostNotificationQueueMessage
    {
        public int JobPostId { get; set; }
    }

    public class NewApplicantQueueMessage
    {
        public int JobPostId { get; set; }
        public int UserApplicationId { get; set; }
    }
}
