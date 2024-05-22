using System.ComponentModel.DataAnnotations;

namespace Sahaj_Yatri.Models.Dto
{
    public class VehicleUpdateDto
    {
        //dto is created because dto are very comman when we work with api because we donot want to direct 
        //expose domain object to the api especillay we are exposing api to third parties

        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        [Range(0, int.MaxValue)]
        public double Price { get; set; }

        [Range(0, 5)]
        public int Rating { get; set; }
        public int Review { get; set; }
        [EmailAddress]

        public string Email { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
        public string Location { get; set; }
    }
}
