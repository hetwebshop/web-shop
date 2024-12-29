namespace API.Entities.Applications
{
    public class UserApplicationPreviousCompanies
    {
        public int Id { get; set; }
        public int UserApplicationId { get; set; }
        public UserApplication UserApplication { get; set; }
        public string CompanyName { get; set; }
        public int StartYear { get; set; }
        public int? EndYear { get; set; }
        public string Position { get; set; }
        public string? Description { get; set; }
    }
}
