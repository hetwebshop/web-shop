using API.Entities.Notification;

namespace API.Entities
{
    public class CompanyNotificationPreferences
    {
        public int Id { get; set; }
        public int UserId { get; set; } //company has user, and based on user we can get company Id
        public User User { get; set; }
        public CompanyNotificationType NotificationType { get; set; }
        public bool IsEnabled { get; set; }
    }
}
