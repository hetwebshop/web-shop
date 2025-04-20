namespace API.DTOs
{
    public class UserAdInfoUpdateRequest
    {
        public string AdTitle { get; set; }
        public string Position { get; set; }
        public int JobTypeId { get; set; }
        public string Biography { get; set; }
        public int JobCategoryId { get; set; }
        public int? YearsOfExperience { get; set; }
        public int? EmploymentStatusId { get; set; }
        public int? EducationLevelId { get; set; }
        public int? EmploymentTypeId { get; set; }
        public int UserAdId { get; set; }
        public string Languages { get; set; }
    }
}
