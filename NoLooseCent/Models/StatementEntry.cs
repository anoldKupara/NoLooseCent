namespace NoLooseCent.Models
{
    public class StatementEntry
    {
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public decimal? Deposit { get; set; }    // null if it is a withdrawal
        public decimal? Withdrawal { get; set; } // null if it is a deposit
        public decimal Balance { get; set; }     // running balance *after* the row
    }
}
