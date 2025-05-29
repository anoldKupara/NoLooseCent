using System.ComponentModel.DataAnnotations;

namespace NoLooseCent.Models
{
    public class Expense
    {
        public int Id { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        public decimal Amount { get; set; }

        public string Purpose { get; set; } // e.g., "Groceries", "Rent"

        [Required]
        public DateTime DateSpent { get; set; }

        // Foreign Key
        [Required]
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }
    }
}