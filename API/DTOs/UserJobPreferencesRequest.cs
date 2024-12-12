namespace API.DTOs
{
    public class UserJobPreferencesRequest
    {
        public string Position { get; set; }
        public string Biography { get; set; }
        public int? JobCategoryId { get; set; }
        public int? JobTypeId { get; set; }
        public int? UserId { get; set; }
        public int? EmploymentTypeId { get; set; }
        public int? YearsOfExperience { get; set; }
        public int? EducationLevelId { get; set; }
        public int? EmploymentStatusId { get; set; }
    }
}
