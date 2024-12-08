using API.Helpers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace API.DTOs
{
    public class UserDto
    {
        public string UserName { get; set; }
        public string Token { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int CityId { get; set; }
        public string City { get; set; }
        public string PhotoUrl { get; set; }
        public string Email { get; set; }
        public bool IsCompany { get; set; } = false;
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPhone { get; set; }
        public string AboutCompany { get; set; }
        public int? CompanyId { get; set; }

        public int Credits { get; set; }
        public Gender Gender { get; set; }

        public string PhoneNumber { get; set; }
        public int? JobCategoryId { get; set; }
        public int? JobTypeId { get; set; }
        public string JobType { get; set; }
        public string JobCategory { get; set; }
        public string Biography { get; set; }
        public string Position { get; set; }
        public IFormFile CvFile { get; set; }
        public string CvFilePath { get; set; }
        public string CvFileName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public List<UserEducationDto> UserEducations { get; set; }
        public List<string> Roles { get; set; }
    }

    public class UserInfoDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int CityId { get; set; }
        public string PhotoUrl { get; set; }
        public bool Exist { get; set; }
    }
}
