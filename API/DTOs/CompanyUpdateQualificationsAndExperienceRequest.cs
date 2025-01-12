namespace API.DTOs
{
    public class CompanyUpdateQualificationsAndExperienceRequest
    {
        public string RequiredSkills { get; set; }
        public string Certifications { get; set; }
        public int? EducationLevelId { get; set; }
        public int? RequiredExperience { get; set; }
    }
}
