namespace Sahaj_Yatri.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string UserName { get; set; } // user who has comment 
        public DateTime RatingDate { get; set; } = DateTime.Now; // comment date 
        public int Rating { get; set; } // number of rating 
        public string Comment { get; set; }
        public string Category { get; set; }
        public string UserProfile { get; set; }
        //public string HotelName  { get; set; } // Reference to Hotel
        public string Name  { get; set; } // Reference to Hotel
        public string  HotelEmail  { get; set; } // Navigation property

        public bool isDeleted { get; set; }
    }
}
