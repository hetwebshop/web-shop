using API.Entities.Applications;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services
{
    public class JobPostNotificationQueueMessage
    {
        public int JobPostId { get; set; }
    }
    public class NewApplicantQueueMessage
    {
        public int JobPostId { get; set; }
        public int UserApplicationId { get; set; }
    }
    public class ApplicantStatusUpdated
    {
        public List<int> UserApplicationIds { get; set; }
    }
    public class NewApplicantPredictionQueueMessage
    {
        public int CompanyJobPostId { get; set; }
        public int UserApplicationId { get; set; }
        //public int? YearsOfExperience { get; set; }
        //public string Position { get; set; }
        //public string CvFileUrl { get; set; }
        //public List<UserApplicationPreviousCompanies> UserPreviousCompanies { get; set; }
        //public List<UserApplicationEducation> UserEducations { get; set; }
        //public string MotivationLetter { get; set; }
    }
    public interface ISendNotificationsQueueClient
    {
        Task SendMessageToUserAsync(JobPostNotificationQueueMessage jobPostNotificationMessage);
        Task SendMessageToCompanyAsync(JobPostNotificationQueueMessage jobPostNotificationMessage);
        Task SendNewApplicantMessageToCompanyAsync(NewApplicantQueueMessage message);
        Task SendNewApplicantPredictionMessageAsync(NewApplicantPredictionQueueMessage message);
        Task SendMessageToUserOnUpdateApplicationStatusAsync(ApplicantStatusUpdated message);
    }
}
