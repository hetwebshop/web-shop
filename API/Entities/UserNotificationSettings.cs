using API.Entities.Notification;

namespace API.Entities
{
    public enum UserNotificationType
    {
        NewInterestingCompanyAdInApp,
        NewInterestingCompanyAdEmail,
    }
    public class UserNotificationSettings
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public UserNotificationType NotificationType { get; set; }
        public bool IsEnabled { get; set; } = true;
    }
}
