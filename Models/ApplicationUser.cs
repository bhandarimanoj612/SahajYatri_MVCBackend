using Microsoft.AspNetCore.Identity;

namespace Sahaj_Yatri.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }

        public string VerificationCode { get; set; }

        public string ProfileImg { get; set; } // for profile image for user
        public bool IsProfileImgDeleted { get; set; } //  for profile image for user

    }
}
 