using System.ComponentModel.DataAnnotations;

namespace NoLooseCent.Models
{
    public class Currency
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } // e.g., "USD Cash", "USD EcoCash"

        [Required]
        public string Code { get; set; } // e.g., "USD-CASH", "USD-ECO"

        // Navigation Properties
        public ICollection<Income> Incomes { get; set; }
        public ICollection<Expense> Expenses { get; set; }
    }
}