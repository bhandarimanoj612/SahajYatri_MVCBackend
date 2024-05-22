//using Sahaj_Yatri.Models.Shared;
using System.ComponentModel.DataAnnotations;

namespace Sahaj_Yatri.Models
{
    //public class Expense : BaseEntity
    public class Expense 
    {
        public int Id { get; set; }

        [Required]
        public string Destination { get; set; }

        public string Title { get; set; }

        public DateTime Date { get; set; }

        public double Amount { get; set; }

        [Required]
        public string UserName { get; set; }
        public bool IsDeleted { get; set; } // New property for soft delete
    }
}
