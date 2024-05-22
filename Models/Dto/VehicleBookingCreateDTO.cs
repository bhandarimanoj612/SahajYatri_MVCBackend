using System;
using System.ComponentModel.DataAnnotations;

namespace Sahaj_Yatri.Models.Dto
{
    public class VehicleBookingCreateDTO
    {
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
        public double TotalPrice { get; set; } // Total price for the booking

        public double PricePerDay { get; set; }

        public string Image { get; set; }
        public int NumberOfDays { get; set; }

        public string StripePaymentIntentID { get; set; } = string.Empty;

        [EmailAddress]

        public string Email { get; set; }

    }
}
