namespace NoLooseCent.ViewModels
{
    public class RecentTransactionViewModel
    {
        public string Type { get; set; }           // "Income" or "Expense"
        public decimal Amount { get; set; }
        public string SourceOrPurpose { get; set; }
        public DateTime Date { get; set; }
        public string CurrencyName { get; set; }
    }
}