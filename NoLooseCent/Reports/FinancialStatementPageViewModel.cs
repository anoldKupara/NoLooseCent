namespace NoLooseCent.Reports
{
    public class FinancialStatementPageViewModel
    {
        public StatementFilterViewModel Filter { get; set; } = new();
        public MonthlyStatementViewModel? Statement { get; set; }
    }
}