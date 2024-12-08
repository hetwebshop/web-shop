namespace API.DTOs
{
    public class UserJobPreferencesRequest
    {
        public string Position { get; set; }
        public string Biography { get; set; }
        public int? JobCategoryId { get; set; }
        public int? JobTypeId { get; set; }
        public int? UserId { get; set; }
    }
}
