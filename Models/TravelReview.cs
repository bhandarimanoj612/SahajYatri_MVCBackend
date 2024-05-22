namespace Sahaj_Yatri.Models
{
    public class TravelReview
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public DateTime RatingDate { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }

        public int? TravelId { get; set; }
        public Travel Travel { get; set; }
    }
}
