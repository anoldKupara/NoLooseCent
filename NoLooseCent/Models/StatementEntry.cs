namespace NoLooseCent.Models
{
    public class StatementEntry
    {
        public DateTime Date { get; set; }
        public string Description { get; set; } = "";
        public decimal? Deposit { get; set; }
        public decimal? Withdrawal { get; set; }
        public decimal Balance { get; set; }
    }
}
