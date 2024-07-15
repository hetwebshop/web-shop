namespace API.DTOs
{
    public class CompanyProfileDto
    {
        public int UserId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPhone { get; set; }
        public string AboutCompany { get; set; }
        public string UserName { get; set; }
        public int CityId { get; set; }
    }
}
