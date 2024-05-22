using System.ComponentModel.DataAnnotations;

namespace Sahaj_Yatri.Models.Dto.Auth
{
    public class ForgotPasswordDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
    }
}
