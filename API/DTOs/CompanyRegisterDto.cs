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
        [Required(ErrorMessage = "Lozinka je obavezna.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Lozinka mora imati između 8 i 100 karaktera.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Lozinka mora sadržavati najmanje jedno veliko slovo, jedno malo slovo, jedan broj i jedan specijalni znak.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Potvrda lozinke je obavezna.")]
        [Compare("Password", ErrorMessage = "Lozinka i potvrda lozinke se ne podudaraju.")]
        public string ConfirmPassword { get; set; }
    }
}
