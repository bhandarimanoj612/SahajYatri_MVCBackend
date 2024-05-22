using System.ComponentModel.DataAnnotations;

namespace Sahaj_Yatri.Models.Dto
{
    public class OfferCreateDTO
    {
        [Required]
        public string Title { get; set; }


        public string Description { get; set; }
    }
}
