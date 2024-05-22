using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sahaj_Yatri.Models
{
    public class VehicleBooking
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }

        public double TotalPrice { get; set; } // Total price for the booking

        public double PricePerDay { get; set; }

        public string Image { get; set; }
        public int NumberOfDays { get; set; }
        public string Status { get; set; }
        [Required]
        public string Name { get; set; }
     

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Number of guests must be greater than 0")]
        public int NumberOfGuests { get; set; }

        public string StripePaymentIntentID { get; set; }

        public string Category { get; set; }


        [EmailAddress]

        public string Email { get; set; }

    }
}
