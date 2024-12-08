namespace API.DTOs
{
    public class UserEducationRequest
    {
        public int? UserId { get; set; }
        public string University { get; set; }
        public string? FieldOfStudy { get; set; }
        public string InstitutionName { get; set; }
        public string Degree { get; set; }
        public int EducationStartYear { get; set; }
        public int? EducationEndYear { get; set; }
        public int? UserEducationId { get; set; }
    }
    public class ApplicantEducationRequest : UserEducationRequest
    {
        public int UserAdId { get; set; }
    }
}
