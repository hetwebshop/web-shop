using API.Helpers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class UserProfileDto
    {
        private DateTime _dateOfBirth;
        public int Id { get; set; }
        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }
        [Required]
        public DateTime DateOfBirth
        {
            get { return _dateOfBirth.Date; }
            set { _dateOfBirth = value.Date; }
        }
        [Required] public Gender Gender { get; set; }
        public string PhoneNumber { get; set; }
        [Required] public string Email { get; set; }
        [Required] public string UserName { get; set; }
        [Required] public int CityId { get; set; }
        public int? JobCategoryId { get; set; }
        public int? JobTypeId { get; set; }
        public string Biography { get; set; }
        public string Position { get; set; }
        public IFormFile CvFile { get; set; }
        public string CvFilePath { get; set; }

        public bool IsCompany { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPhone { get; set; }
        public string AboutCompany { get; set; }
        public int? CompanyId { get; set; }

        public List<UserEducationDto> UserEducations { get; set; }
    }
}
