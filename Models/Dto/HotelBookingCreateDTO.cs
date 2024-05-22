using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sahaj_Yatri.Models.Dto
{
    public class HotelBookingCreateDTO
    {

        [Required]
        public string UserName { get; set; }


        [Required]
      // Specify that only date should be used
        public DateTime StartDate { get; set; }

        [Required]
       // Specify that only date should be used
        public DateTime EndDate { get; set; }


        [Range(1, int.MaxValue, ErrorMessage = "Number of guests must be greater than 0")]
        public int NumberOfGuests { get; set; }

        public int NumberOfDays { get; set; }
        public double TotalPrice { get; set; } // Total price for the booking

        public double PricePerDay { get; set; }

        public string Image { get; set; }

        public string StripePaymentIntentID { get; set; }

        public string Name { get; set; }

        [EmailAddress]

        public string Email { get; set; }

    }
}
