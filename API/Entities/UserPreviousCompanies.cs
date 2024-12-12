using System;

namespace API.Entities
{
    public class UserPreviousCompanies
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string CompanyName { get; set; }
        public int StartYear { get; set; }
        public int? EndYear { get; set; }
        public string Position { get; set; }
        public string? Description { get; set; }
    }
}
