using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class CompanyRegisterDto
    {
        [Required] public string CompanyName { get; set; }
        [Required] public int CityId { get; set; }
        public string PhoneNumber { get; set; }
        [Required] public string Email { get; set; }
        [Required] public string UserName { get; set; }
        [Required] public string Address { get; set; }
        [Required]
        [StringLength(12, MinimumLength = 4)]
        public string Password { get; set; }
    }
}
