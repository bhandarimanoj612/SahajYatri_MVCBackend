using System.ComponentModel.DataAnnotations;

namespace Sahaj_Yatri.Models
{
    public class Offer
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
    }
}
