//using Sahaj_Yatri.Models.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sahaj_Yatri.Models
{
    //public class Trip : BaseEntity
    public class Trip 
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Destination { get; set; }

        public string City { get; set; }

        public DateTime Date { get; set; }

        [Required]
        public string UserName { get; set; }

        public bool IsDeleted { get; set; } // New property for soft delete

    }
}
