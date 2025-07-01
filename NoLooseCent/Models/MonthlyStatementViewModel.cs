namespace NoLooseCent.Models
{
    public class MonthlyStatementViewModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName => new DateTime(Year, Month, 1).ToString("MMMM yyyy");

        public string CurrencyCode { get; set; } = "";

        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }

        public IList<StatementEntry> Entries { get; set; } = new List<StatementEntry>();
    }
}
