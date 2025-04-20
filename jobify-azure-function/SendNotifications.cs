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
using Azure.Storage.Blobs;

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

        [Function("SendEmailNotification")]
        public async Task SendEmailNotificationFunction([QueueTrigger("jobify-notification-queue", Connection = "AzureWebJobsStorage")] QueueMessage message)
        {
            var notificationMessage = JsonConvert.DeserializeObject<NotificationEventMessage>(message.MessageText);

            _logger.LogInformation("Processing notification");

            if(notificationMessage.NotificationType == null)
            {
                _logger.LogError("Notification type should not be null!");
                return;
            }

            var notificationType = notificationMessage.NotificationType;
            switch (notificationType)
            {
                case NotificationType.SendUserNotificationsOnCreateCompanyAd:
                    if(notificationMessage.JobPostId == null)
                    {
                        _logger.LogWarning("JobPostId must be provided when creating notification of type SendUserNotificationsOnCreateCompanyAd");
                        return;
                    }
                    await SendUserNotificationsOnCreateCompanyAd(notificationMessage.JobPostId);
                    break;
                case NotificationType.SendCompanyNotificationOnCreateUserAd:
                    if (notificationMessage.JobPostId == null)
                    {
                        _logger.LogWarning("JobPostId must be provided when creating notification of type SendCompanyNotificationOnCreateUserAd");
                        return;
                    }
                    await SendCompanyNotificationOnCreateUserAd(notificationMessage.JobPostId);
                    break;

                case NotificationType.SendCompanyNotificationOnNewUserApplication:
                    if(notificationMessage.JobPostId == null || notificationMessage.UserApplicationIds == null || !notificationMessage.UserApplicationIds.Any())
                    {
                        _logger.LogWarning("JobPostId and UserApplicationId must be provided when creating notification of type SendCompanyNotificationOnNewUserApplication");
                        return;
                    }
                    var userApplicationId = notificationMessage.UserApplicationIds.First();//in this notification type we will have only one item in the list
                    var jobPostId = notificationMessage.JobPostId;
                    await SendCompanyNotificationOnNewUserApplication(jobPostId, userApplicationId);
                    break;
                case NotificationType.SendFeedbackToUserOnCompanyUpdateStatus:
                    if(notificationMessage.UserApplicationIds == null || !notificationMessage.UserApplicationIds.Any())
                    {
                        _logger.LogWarning("UserApplicationIds must be provided when creating notification of type SendFeedbackToUserOnCompanyUpdateStatus");
                        return;
                    }
                    await SendFeedbackToUserOnCompanyUpdateStatus(notificationMessage.UserApplicationIds);
                    break;

                default:
                    _logger.LogWarning("Unknown notification type: {NotificationType}", notificationMessage.NotificationType);
                    break;
            }
        }


        private async Task SendUserNotificationsOnCreateCompanyAd(int jobPostId)
        {
            _logger.LogInformation($"Processing new company ad creation notification for job post: {jobPostId}");

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Get the job post by ID using Dapper
                var query = "SELECT Id, JobCategoryId FROM CompanyJobPosts WHERE Id = @JobPostId";
                var newItem = await connection.QueryFirstOrDefaultAsync<dynamic>(query, new { JobPostId = jobPostId });
                var category = await connection.QueryFirstOrDefaultAsync<string>(
                    "SELECT Name FROM JobCategories WHERE Id = @JobCategoryId",
                    new { JobCategoryId = newItem.JobCategoryId }
                );
                if (newItem != null)
                {
                    // Get users with similar interests
                    var usersWithSimilarInterestQuery = "SELECT Id, Email FROM AspNetUsers WHERE JobCategoryId = @JobCategoryId";
                    var usersWithSimilarInterest = await connection.QueryAsync<dynamic>(usersWithSimilarInterestQuery, new { JobCategoryId = newItem.JobCategoryId });

                    if (usersWithSimilarInterest.Any())
                    {
                        // Get user notification settings using Dapper
                        var userNotifSettingsQuery = "SELECT * FROM UserNotificationSettings WHERE IsEnabled = 1 AND UserId IN @UserIds";
                        var userNotifSettings = await connection.QueryAsync<dynamic>(userNotifSettingsQuery, new { UserIds = usersWithSimilarInterest.Select(u => u.Id).ToArray() });

                        // In-app notifications using Dapper
                        foreach (var userNotif in userNotifSettings.Where(r => r.NotificationType == 0))
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
                            .Where(setting => setting.NotificationType == 1)
                            .Select(async userNotif =>
                            {
                                var user = usersWithSimilarInterest.FirstOrDefault(u => u.Id == userNotif.UserId);
                                if (user != null)
                                {
                                    string adUrl = UIBaseUrl + $"company-ad-details/{newItem.Id}";
                                    string messageBody = $@"
                                    <p style='color: black;'>Poštovani <strong>{user.Email}</strong>,</p>
                                    <p style='color: black;'>Kreiran je novi oglas za posao u kategoriji: <strong>{category}</strong>.</p>
                                    <p style='color: black;'>Pogledajte detalje i prijavite se za ovu poslovnu priliku.</p>
                                    <p style='text-align: start;'>
                                        <a href='{adUrl}' style='display: inline-block; padding: 10px 20px; background-color: #66023C; color: #ffffff; text-decoration: none; border-radius: 5px;'>Pogledajte Oglas</a>
                                    </p>
<p style='color: black;'>Ako dugme nije klikabilno, možete otvoriti sljedeći URL: <a href='{adUrl}'>{adUrl}</a></p>
";
                                    var emailTemplate = EmailTemplateHelper.GenerateEmailTemplate("", messageBody, _configuration);

                                    var subject = "Poslovnioglasi - Novi oglas koji bi vas mogao zanimati";
                                    await _emailService.SendEmailWithTemplateAsync(user.Email, subject, emailTemplate);
                                }
                            }).ToList();

                        // Execute all email sending tasks asynchronously
                        await Task.WhenAll(emailTasks);
                    }
                }
            }
        }

        private async Task SendCompanyNotificationOnCreateUserAd(int jobPostId)
        {
            _logger.LogInformation($"Processing new user ad created notification for job post: {jobPostId}");

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Get the job post by ID
                var query = "SELECT Id, JobCategoryId FROM UserJobPosts WHERE Id = @JobPostId";
                var newItem = await connection.QueryFirstOrDefaultAsync<dynamic>(query, new { JobPostId = jobPostId });

                if (newItem != null)
                {
                    // Get companies that are interested in the job category
                    var companiesToNotify = await connection.QueryAsync<dynamic>(
                        "SELECT UserId FROM CompanyJobCategoryInterests WHERE JobCategoryId = @JobCategoryId",
                        new { JobCategoryId = newItem.JobCategoryId }
                    );

                    var companiesNotifSettings = await connection.QueryAsync<dynamic>(
                        "SELECT * FROM CompanyNotificationPreferences WHERE IsEnabled = 1 AND UserId IN @UserIds",
                        new { UserIds = companiesToNotify.Select(c => c.UserId).ToArray() }
                    );

                    var category = await connection.QueryFirstOrDefaultAsync<string>(
                        "SELECT Name FROM JobCategories WHERE Id = @JobCategoryId",
                        new { JobCategoryId = newItem.JobCategoryId }
                    );

                    // In-app notifications
                    foreach (var companiesSetting in companiesNotifSettings.Where(r => r.NotificationType == 2))
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

                    var userIds = companiesToNotify.Select(c => (int)c.UserId).ToList();
                    if (userIds.Any())
                    {
                        var companyUsers = await connection.QueryAsync<dynamic>(
                            "SELECT Id, Email FROM AspNetUsers WHERE Id IN @UserIds",
                            new { UserIds = userIds }
                        );

                        // Send email notifications
                        var emailTasks = companiesNotifSettings
                            .Where(setting => setting.NotificationType == 3)
                            .Select(async companiesSetting =>
                            {
                                var user = companyUsers.FirstOrDefault(u => u.Id == companiesSetting.UserId);
                                if (user != null)
                                {
                                    string adUrl = UIBaseUrl + $"ad-details/{newItem.Id}";
                                    string messageBody = $@"
                            <p style='color: black;'>Poštovani <strong>{user.Email}</strong>,</p>
                            <p style='color: black;'>Kreiran je novi korisnički oglas za posao u kategoriji: <strong>{category}</strong>.</p>
                            <p style='color: black;'>Pogledajte detalje i kontaktirajte korisnika kako biste saznali više.</p>
                            <p style='text-align: start;'>
                                <a href='{adUrl}' style='display: inline-block; padding: 10px 20px; background-color: #66023C; color: #ffffff; text-decoration: none; border-radius: 5px;'>Pogledajte Oglas</a>
                            </p>
<p style='color: black;'>Ako dugme nije klikabilno, možete otvoriti sljedeći URL: <a href='{adUrl}'>{adUrl}</a></p>
";

                                    var emailTemplate = EmailTemplateHelper.GenerateEmailTemplate("", messageBody, _configuration);

                                    var subject = "Poslovnioglasi - Novi oglas koji bi vas mogao zanimati";
                                    await _emailService.SendEmailWithTemplateAsync(user.Email, subject, emailTemplate);
                                }
                            }).ToList();

                        // Execute all email sending tasks asynchronously
                        await Task.WhenAll(emailTasks);
                    }
                }
            }
        }

        private async Task SendCompanyNotificationOnNewUserApplication(int jobPostId, int userApplicationId)
        {
            _logger.LogInformation($"Processing new applicant notification for job post: {jobPostId}");

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Get the job post by ID
                var query = "SELECT Id, EmailForReceivingApplications, CompanyName, Position, SubmittingUserId FROM CompanyJobPosts WHERE Id = @JobPostId";
                var companyJobPost = await connection.QueryFirstOrDefaultAsync<dynamic>(query, new { JobPostId = jobPostId });

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
                        var applicationUrl = UIBaseUrl + $"company-settings/candidate-details/{userApplicationId}";

                        if (emailNotifEnabled != null)
                        {
                            var userEmail = companyJobPost.EmailForReceivingApplications;
                            var companyName = companyJobPost.CompanyName;
                            var position = companyJobPost.Position;

                            var cvFilePathQuery = "SELECT CvFilePath, CvFileName FROM UserApplications WHERE Id = @UserApplicationId";
                            var userApplication = await connection.QueryFirstOrDefaultAsync<dynamic>(cvFilePathQuery, new { UserApplicationId = userApplicationId });
                            if (userApplication == null)
                                return;
                            string messageBody = $@"
    <p style='color: black;'>Dragi <strong>{companyName}</strong>,</p>
    <p style='color: black;'>Dobili ste novu prijavu za poziciju: <strong>{position}</strong>.</p>
";
                            // Conditional message for CV attachment
                            if (string.IsNullOrEmpty(userApplication.CvFilePath))
                            {
                                messageBody += @"
                                    <p style='color: black;'>Korisnik nije priložio CV datoteku prilikom apliciranja na oglas.</p>";
                            }
                            else
                            {
                                messageBody += @"
                                    <p style='color: black; font-weight: bold;'>CV datoteka se nalazi u prilogu.</p>
                                    <p style='color: black; font-size: smaller; font-weight: bold;'>Molimo vas da obrišete CV datoteku najkasnije 90 dana nakon isteka vašeg oglasa, osim u situaciji ako ste u međuvremenu s aplikantom ostvarili radni odnos.</p>";
                            }

                            // Add the link to view the application details
                            messageBody += $@"
    <p style='color: black;'>Da biste pogledali sve detalje prijave, pritisnite na dugme ispod:</p>
    <p style='text-align: start;'>
        <a href='{applicationUrl}' style='display: inline-block; padding: 10px 20px; background-color: #66023C; color: #ffffff; text-decoration: none; border-radius: 5px;'>Detalji Prijave</a>
    </p>
    <p style='color: black;'>Ako dugme nije klikabilno, možete otvoriti sljedeći URL: <a href='{applicationUrl}'>{applicationUrl}</a></p>";
                            var emailTemplate = EmailTemplateHelper.GenerateEmailTemplate("", messageBody, _configuration);

                            try
                            {
                                var subject = "Poslovnioglasi - Nova prijava na vaš oglas za posao";
                                await _emailService.SendEmailWithTemplateAsync(userEmail, subject, emailTemplate, userApplication.CvFilePath, userApplication.CvFileName);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError("Došlo je do greške prilikom slanja maila.", ex.Message);
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
                                Link = applicationUrl,
                                Message = $"Kreirana je nova aplikacija za posao na vašem oglasu {companyJobPost.Position}"
                            };

                            var insertNotificationQuery = "INSERT INTO Notifications (UserId, CreatedAt, IsRead, Link, Message) VALUES (@UserId, @CreatedAt, @IsRead, @Link, @Message)";
                            await connection.ExecuteAsync(insertNotificationQuery, notification);

                        }
                    }
                }
            }
        }

        private async Task SendFeedbackToUserOnCompanyUpdateStatus(List<int> userApplicationIds)
        {
            _logger.LogInformation($"Processing feedback to user on company update status");

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var userApplicationsQuery = "SELECT Id, Email, Feedback, ApplicationStatusId, MeetingDateTime, SubmittingUserId, CompanyJobPostId FROM UserApplications WHERE Id IN @UserApplicationIds";
                var userApplications = await connection.QueryAsync<dynamic>(userApplicationsQuery, new
                {
                    UserApplicationIds = userApplicationIds
                });
                if (userApplications != null && userApplications.Any())
                {
                    // Get the job post by ID
                    var query = "SELECT Id, CompanyName, EmailForReceivingApplications, Position FROM CompanyJobPosts WHERE Id = @JobPostId";
                    var companyJobPost = await connection.QueryFirstOrDefaultAsync<dynamic>(query, new { JobPostId = userApplications.First().CompanyJobPostId });

                    if (companyJobPost != null)
                    {
                        foreach(var userApplication in userApplications)
                        {
                            var userEmail = userApplication.Email;
                            var companyName = companyJobPost.CompanyName;
                            var companyEmail = companyJobPost.EmailForReceivingApplications;
                            var position = companyJobPost.Position;
                            var feedback = userApplication.Feedback;
                            bool isMeetingScheduled = userApplication.ApplicationStatusId == 3 ? true : false;
                            var applicationUrl = UIBaseUrl + $"user-settings/my-applications/{userApplication.Id}";
                            var title = $"Kompanija {companyName} vam je odgovorila na prijavu za poziciju {companyJobPost.Position}.";
                            var meetingDateTimeUTC = userApplication.MeetingDateTime;
                            var messageToUser = feedback;
                            if(isMeetingScheduled && meetingDateTimeUTC != null)
                            {
                                TimeZoneInfo cetTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
                                var meetingDateTimeCET = TimeZoneInfo.ConvertTimeFromUtc(meetingDateTimeUTC, cetTimeZone);
                                var formattedDateTime = meetingDateTimeCET.ToString("dd.MM.yyyy HH:mm");
                                messageToUser = $"Vaš intervju je zakazan za {formattedDateTime} (CET).";
                            }

                            string messageBody = $@"
            <h4 style='color: black; font-weight: normal;'>Naslov: {title}</h4>
            <p style='color: black;'>Poruka: 
                {messageToUser}</p>
            <p style='color: black;'>Email poslodavca: {companyEmail}</p>
            <p style='text-align: start;'>
                <a href='{applicationUrl}' style='display: inline-block; padding: 10px 20px; background-color: #66023C; color: #ffffff; text-decoration: none; border-radius: 5px;'>Više detalja</a>
            </p>
<p style='color: black;'>Ako dugme nije klikabilno, možete otvoriti sljedeći URL: <a href='{applicationUrl}'>{applicationUrl}</a></p>
";

                            var emailTemplate = EmailTemplateHelper.GenerateEmailTemplate("", messageBody, _configuration);

                            try
                            {
                                var subject = $"Poslovnioglasi – Odgovor na vašu prijavu za poziciju {companyJobPost.Position}";
                                await _emailService.SendEmailWithTemplateAsync(userEmail, subject, emailTemplate);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError("Došlo je do greške prilikom slanja maila.", ex.Message);
                            }

                            var notification = new
                            {
                                UserId = userApplication.SubmittingUserId.ToString(),
                                CreatedAt = DateTime.UtcNow,
                                IsRead = false,
                                Link = applicationUrl,
                                Message = $"Poslodavac je odgovorio na vašu prijavu za poziciju {companyJobPost.Position}"
                            };

                            var insertNotificationQuery = "INSERT INTO Notifications (UserId, CreatedAt, IsRead, Link, Message) VALUES (@UserId, @CreatedAt, @IsRead, @Link, @Message)";
                            await connection.ExecuteAsync(insertNotificationQuery, notification);
                        }
                    }
                }
            }
        }
    }

    public enum NotificationType
    {
        SendUserNotificationsOnCreateCompanyAd,
        SendCompanyNotificationOnCreateUserAd,
        SendCompanyNotificationOnNewUserApplication,
        SendFeedbackToUserOnCompanyUpdateStatus
    }

    public class NotificationEventMessage
    {
        public int JobPostId { get; set; }
        public List<int> UserApplicationIds { get; set; }
        public NotificationType NotificationType { get; set; }
    }
}
