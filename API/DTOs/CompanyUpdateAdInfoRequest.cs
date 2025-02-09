namespace API.DTOs
{
    public class CompanyUpdateAdInfoRequest
    {
        public int CityId { get; set; }
        public string Position { get; set; }
        public int JobCategoryId { get; set; }
        public int JobTypeId { get; set; }
        public string JobDescription { get; set; }
        public string EmailForReceivingApplications { get; set; }
        public string CompanyName { get; set; }
    }
}
