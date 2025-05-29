using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace NoLooseCent.Models
{
    public class Income
    {
        public int Id { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        public decimal Amount { get; set; }

        public string Source { get; set; } // e.g., "Salary", "Side Hustle"

        [Required]
        public DateTime DateReceived { get; set; }

        // Foreign Key
        [Required]
        public int CurrencyId { get; set; }
        [BindNever]
        public Currency Currency { get; set; }
    }
}