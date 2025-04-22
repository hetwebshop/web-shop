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
        public string UserName { get; set; }

        [Required(ErrorMessage = "Lozinka je obavezna.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Lozinka mora imati između 8 i 100 karaktera.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_\+\-=\[\]{}|:;""'<>,\.?/])[A-Za-z\d!@#$%^&*()_\+\-=\[\]{}|:;""'<>,\.?/]{8,}$",
            ErrorMessage = "Lozinka mora sadržavati najmanje jedno veliko slovo, jedno malo slovo, jedan broj i jedan specijalni znak.")]
        public string Password { get; set; }


        [Required(ErrorMessage = "Potvrda lozinke je obavezna.")]
        [Compare("Password", ErrorMessage = "Lozinka i potvrda lozinke se ne podudaraju.")]
        public string ConfirmPassword { get; set; }
        [Required]
        public bool TermsAccepted { get; set; }

        [Required]
        public string CaptchaToken { get; set; }
    }
}