using API.Helpers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class UserProfileDto
    {
        public int Id { get; set; }
        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }
        [Required] public DateTime DateOfBirth { get; set; }
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

        public List<UserEducationDto> UserEducations { get; set; }
    }
}
