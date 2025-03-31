using API.Entities.Applications;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services
{
    //ENUM VALUES MUST BE THE SAME AS IN JOBIFY NOTIFICATIONS AZURE FUNCTION
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
    public class NewApplicantPredictionQueueMessage
    {
        public int CompanyJobPostId { get; set; }
        public int UserApplicationId { get; set; }
        public string Position { get; set; }
    }
    public interface ISendNotificationsQueueClient
    {
        Task SendMessageToUserAsync(NotificationEventMessage message);
        Task SendMessageToCompanyAsync(NotificationEventMessage message);
        Task SendNewApplicantMessageToCompanyAsync(NotificationEventMessage message);
        Task SendNewApplicantPredictionMessageAsync(NewApplicantPredictionQueueMessage message);
        Task SendMessageToUserOnUpdateApplicationStatusAsync(NotificationEventMessage message);
    }
}
