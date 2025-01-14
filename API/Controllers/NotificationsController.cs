using API.Data;
using API.DTOs;
using API.Entities;
using API.Entities.Notification;
using API.Extensions;
//using API.Services.NotificationsHub;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    public class NotificationsController : BaseController
    {
        private readonly DataContext _dbContext;
        private readonly ILogger<NotificationsController> _logger;

        //private readonly IHubContext<NotificationsHub> _hubContext;
        public NotificationsController(DataContext dbContext, ILogger<NotificationsController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
            //_hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = HttpContext.User.GetUserId().ToString();
            var notifications = await _dbContext.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(notifications);
        }

        [HttpGet("unread")]
        public async Task<IActionResult> GetUnreadNotificationsCount()
        {
            var userId = HttpContext.User.GetUserId();
            var notifications = await _dbContext.Notifications
                .Where(n => n.UserId == userId.ToString() && n.IsRead == false).ToListAsync();
            return Ok(notifications.Count());
        }

        [HttpPost("mark-as-read/{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = HttpContext.User.GetUserId();
            var notification = await _dbContext.Notifications.FirstOrDefaultAsync(r => r.Id == id);
            if(notification != null)
            {
                if (notification.UserId != userId.ToString())
                    return Forbid("Niste autorizovani za akciju!");
                notification.IsRead = true;

                _dbContext.Update(notification);
                _dbContext.SaveChanges();
                return Ok(true);
            }
            return BadRequest(false);
        }

        [HttpPost("mark-all-as-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = HttpContext.User.GetUserId();
            var notifications = await _dbContext.Notifications
                .Where(r => r.UserId == userId.ToString() && !r.IsRead)
                .ToListAsync();

            if (!notifications.Any())
                return Ok(false); // No notifications to update

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }
            await _dbContext.SaveChangesAsync();

            return Ok(true);
        }


        [HttpPost]
        public async Task<IActionResult> SendNotification(string userId, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            _dbContext.Notifications.Add(notification);
            await _dbContext.SaveChangesAsync();

            //await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", notification);

            return Ok(new { Status = "Notification Sent" });
        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveSettings([FromBody] CompanyNotificationSettingsDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid notification settings");
            }

            try
            {
                var userId = HttpContext.User.GetUserId(); // Assumes GetUserId is an extension method or helper to get the user ID.

                // Prepare the new job category interests
                var companyJobInterests = dto.SelectedJobCategories.Select(selectedCategory => new CompanyJobCategoryInterests
                {
                    JobCategoryId = selectedCategory,
                    UserId = userId
                }).ToList();

                // Remove existing job category interests for the user
                var existingInterests = _dbContext.CompanyJobCategoryInterests
                    .Where(r => r.UserId == userId);

                if (existingInterests.Any())
                {
                    _dbContext.CompanyJobCategoryInterests.RemoveRange(existingInterests);
                }

                // Add new job category interests
                _dbContext.CompanyJobCategoryInterests.AddRange(companyJobInterests);

                // Prepare the updated notification preferences
                var existingCompanyNotifications = _dbContext.CompanyNotificationPreferences
                    .Where(r => r.UserId == userId)
                    .ToList();

                foreach (var companyNotification in existingCompanyNotifications)
                {
                    switch (companyNotification.NotificationType)
                    {
                        case CompanyNotificationType.newApplicantInApp:
                            companyNotification.IsEnabled = dto.InAppApplicantNotification;
                            break;
                        case CompanyNotificationType.newApplicantEmail:
                            companyNotification.IsEnabled = dto.EmailApplicantNotification;
                            break;
                        case CompanyNotificationType.newInsterestingUserAdEmail:
                            companyNotification.IsEnabled = dto.EmailJobNotification;
                            break;
                        case CompanyNotificationType.newInterestingUserAdInApp:
                            companyNotification.IsEnabled = dto.InAppJobNotification;
                            break;
                    }
                }

                // Update the preferences
                _dbContext.CompanyNotificationPreferences.UpdateRange(existingCompanyNotifications);

                // Save changes to the database
                await _dbContext.SaveChangesAsync();

                return Ok(true);
            }
            catch (Exception ex)
            {
                // Log the exception details
                _logger.LogError(ex, "An error occurred while saving the settings");

                // Return an error response
                return StatusCode(500, "An error occurred while saving the settings");
            }
        }

        [HttpPost("save-user")]
        public async Task<IActionResult> SaveUserNotificationSettings([FromBody] UserNotificationSettingsDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid notification settings");
            }

            try
            {
                var userId = HttpContext.User.GetUserId(); // Assumes GetUserId is an extension method or helper to get the user ID.

                var userExistingNotifSettings = _dbContext.UserNotificationSettings
                    .Where(r => r.UserId == userId)
                    .ToList();  // Ensure this is a list to use in memory

                if (userExistingNotifSettings.Any())
                {
                    // Update in-app notification setting
                    var inAppSettings = userExistingNotifSettings
                        .FirstOrDefault(r => r.NotificationType == UserNotificationType.NewInterestingCompanyAdInApp);

                    if (inAppSettings != null)
                    {
                        inAppSettings.IsEnabled = dto.InAppNotifications;
                    }

                    // Update email notification setting
                    var emailSetting = userExistingNotifSettings
                        .FirstOrDefault(r => r.NotificationType == UserNotificationType.NewInterestingCompanyAdEmail);

                    if (emailSetting != null)
                    {
                        emailSetting.IsEnabled = dto.EmailNotifications;
                    }

                    // Only update and save if changes were made
                    _dbContext.UserNotificationSettings.UpdateRange(userExistingNotifSettings);

                    // Save changes to the database
                    await _dbContext.SaveChangesAsync();
                }

                return Ok(true);
            }
            catch (Exception ex)
            {
                // Log the exception details
                _logger.LogError(ex, "An error occurred while saving the settings");

                // Return an error response
                return StatusCode(500, "An error occurred while saving the settings");
            }
        }


        [HttpGet("company-notification-settings")]
        public async Task<IActionResult> GetCompanyNotificationSettings()
        {
            var userId = HttpContext.User.GetUserId();
            var companyNotificationSettings = await _dbContext.CompanyNotificationPreferences
                .Where(n => n.UserId == userId).ToListAsync();
            var companyCategoriesOfInterests = await _dbContext.CompanyJobCategoryInterests.Where(r => r.UserId == userId).ToListAsync();
            if (!companyNotificationSettings.Any())
            {
                return Ok(new CompanyNotificationSettingsDto()
                {
                    EmailApplicantNotification = false,
                    EmailJobNotification = false,
                    InAppJobNotification = false,
                    InAppApplicantNotification = false,
                    SelectedJobCategories = new List<int>()
                });
            }
            var companyNotifDto = new CompanyNotificationSettingsDto()
            {
                EmailApplicantNotification = companyNotificationSettings.FirstOrDefault(r => r.NotificationType == CompanyNotificationType.newApplicantEmail).IsEnabled,
                EmailJobNotification = companyNotificationSettings.FirstOrDefault(r => r.NotificationType == CompanyNotificationType.newInsterestingUserAdEmail).IsEnabled,
                InAppJobNotification = companyNotificationSettings.FirstOrDefault(r => r.NotificationType == CompanyNotificationType.newInterestingUserAdInApp).IsEnabled,
                InAppApplicantNotification = companyNotificationSettings.FirstOrDefault(r => r.NotificationType == CompanyNotificationType.newApplicantInApp).IsEnabled,
                SelectedJobCategories = companyCategoriesOfInterests?.Select(r => r.JobCategoryId).ToList()
            };
            return Ok(companyNotifDto);
        }

        [HttpGet("user-notification-settings")]
        public async Task<IActionResult> GetUserNotificationSettings()
        {
            var userId = HttpContext.User.GetUserId();
            var userNotif = await _dbContext.UserNotificationSettings
                .Where(n => n.UserId == userId).ToListAsync();
            var companyNotifDto = new UserNotificationSettingsDto()
            {
                EmailNotifications = userNotif.FirstOrDefault(r => r.NotificationType == UserNotificationType.NewInterestingCompanyAdEmail)?.IsEnabled ?? false,
                InAppNotifications = userNotif.FirstOrDefault(r => r.NotificationType == UserNotificationType.NewInterestingCompanyAdInApp)?.IsEnabled ?? false,
            };
            return Ok(companyNotifDto);
        }
    }
}
