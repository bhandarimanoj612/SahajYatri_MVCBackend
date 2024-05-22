using System.ComponentModel.DataAnnotations;

namespace Sahaj_Yatri.Models.Dto.Auth
{
    public class ResetPasswordDTO
    {
        //[Required(ErrorMessage = "Token is required")]
        //public string Token { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        // Add this property for OTP
        [Required(ErrorMessage = "OTP is required")]
        public string OTP { get; set; }
    }
}
