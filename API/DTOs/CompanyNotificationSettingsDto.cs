using System.Collections.Generic;

namespace API.DTOs
{
    public class CompanyNotificationSettingsDto
    {
        public bool EmailApplicantNotification { get; set; }
        public bool InAppApplicantNotification { get; set; }
        public bool EmailJobNotification { get; set; }
        public bool InAppJobNotification { get; set; }
        public List<int> SelectedJobCategories { get; set; }
    }
}
