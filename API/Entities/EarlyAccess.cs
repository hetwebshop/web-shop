using System;

namespace API.Entities
{
    public class EarlyAccess
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsCompany { get; set; }
        public DateTime SubmittedOn { get; set; }
    }
}
