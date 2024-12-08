namespace API.DTOs
{
    public class CompanyBaseInfoRequest
    {
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public int CityId { get; set; }
        public string FirstName { get; set; } //contact first name
        public string LastName { get; set; }
        public string AboutUs { get; set; }
        public string PhoneNumber { get; set; }
    }
}
