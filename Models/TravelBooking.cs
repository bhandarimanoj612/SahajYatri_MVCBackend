using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Sahaj_Yatri.Models
{
    public class TravelBooking
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Number of guests must be greater than 0")]
        public int NumberOfGuests { get; set; }

        public string StripePaymentIntentID { get; set; }

        public double TotalPrice { get; set; } // Total price for the booking

        public double PricePerDay { get; set; }

        public string Image { get; set; }
        public int NumberOfDays { get; set; 
        }
        public string Category { get; set; }

        public string Status { get; set; }


        [EmailAddress]

        public string Email { get; set; }

    }
}
