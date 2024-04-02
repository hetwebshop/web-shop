using API.Helpers;
using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class UserRegisterDto
    {
        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }
        [Required] public int CityId { get; set; }
        [Required] public DateTime DateOfBirth { get; set; }
        [Required] public Gender Gender { get; set; }
        public string PhoneNumber { get; set; }
        [Required] public string Email { get; set; }
        [Required] public string UserName { get; set; }

        [Required]
        [StringLength(12, MinimumLength = 4)]
        public string Password { get; set; }
    }
}