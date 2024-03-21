using API.Helpers;
using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class UserProfileDto
    {
        public int Id { get; set; }
        [Required] public string Name { get; set; }
        [Required] public DateTime DateOfBirth { get; set; }
        [Required] public Gender Gender { get; set; }
        public string PhoneNumber { get; set; }
        [Required] public string Email { get; set; }
        [Required] public string UserName { get; set; }
    }
}
