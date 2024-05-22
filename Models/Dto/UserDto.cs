using System.ComponentModel.DataAnnotations;

namespace Sahaj_Yatri.Models.Dto
{
    public class UserDto
    {
        [Required]
        [StringLength(15, MinimumLength = 3, ErrorMessage ="Name must be atleast {2} and maximun {1} character")]
        public string Name {  get; set; }

    }
}
