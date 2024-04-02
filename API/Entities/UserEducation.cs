namespace API.Entities
{
    public class UserEducation
    {
        public int Id { get; set; }
        public string Degree { get; set; }
        public string University { get; set; }
        public string InstitutionName { get; set; }
        public string FieldOfStudy { get; set; }
        public int EducationStartYear { get; set; }
        public int EducationEndYear { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
