using API.Helpers;
using System;

namespace API.DTOs
{
    public class UserAdBaseInfoRequest
    {
        public int UserAdId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public int CityId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string Email { get; set; }
    }
}
