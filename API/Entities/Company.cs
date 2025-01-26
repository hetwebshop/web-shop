using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace API.Entities
{
    public class Company
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public int CityId { get; set; }
        public City City { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; } 
        public string AboutUs { get; set; }
        public string PhotoUrl { get; set; }

        //public int ContactPersonId { get; set; }
        //public User ContactPerson { get; set; }
    }
}
