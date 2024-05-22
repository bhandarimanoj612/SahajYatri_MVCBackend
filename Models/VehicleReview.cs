namespace Sahaj_Yatri.Models
{
    public class VehicleReview
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public DateTime RatingDate { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }

        public int? VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }

    }
}
