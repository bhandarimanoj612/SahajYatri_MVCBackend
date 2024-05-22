using System.ComponentModel.DataAnnotations;

namespace Sahaj_Yatri.Models
{
    public class Vehicle
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        // Include other properties as needed
        [Range(0, int.MaxValue)]
        public double Price { get; set; }

        [Range(0, 5)]
        public int Rating { get; set; }
        public int Review { get; set; }
        [EmailAddress]

        public string Email { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }

        public string Image { get; set; }


        public string Location { get; set; }

        public string Category { get; set; } // Add Category property
    }
}
