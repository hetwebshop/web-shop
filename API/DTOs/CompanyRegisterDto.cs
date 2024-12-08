using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class CompanyRegisterDto
    {
        [Required] public string CompanyName { get; set; }
        [Required] public int CityId { get; set; }
        [Required]public string PhoneNumber { get; set; }
        [Required] public string Email { get; set; }
        public string UserName { get; set; }
        [Required] public string Address { get; set; }
        [Required] public string FirstName { get; set; }//Contact first name
        [Required] public string LastName { get; set; }//Contact last name
        [Required]
        [StringLength(12, MinimumLength = 4)]
        public string Password { get; set; }
    }
}
