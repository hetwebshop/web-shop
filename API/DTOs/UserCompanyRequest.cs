namespace API.DTOs
{
    public class UserCompanyRequest
    {
        public int? UserId { get; set; }
        public string CompanyName { get; set; }
        public string Position { get; set; }
        public string Description { get; set; }
        public int StartYear { get; set; }
        public int? EndYear { get; set; }
        public int? UserCompanyId { get; set; }
    }
    public class ApplicantCompanyRequst : UserCompanyRequest
    {
        public int UserAdId { get; set; }
    }
}
