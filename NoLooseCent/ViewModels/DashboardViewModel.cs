namespace NoLooseCent.ViewModels
{
    public class DashboardViewModel
    {
        // USD
        public decimal TotalUsdIncome { get; set; }
        public decimal TotalUsdExpense { get; set; }
        public decimal UsdBalance => TotalUsdIncome - TotalUsdExpense;

        // ZWL
        public decimal TotalZwlIncome { get; set; }
        public decimal TotalZwlExpense { get; set; }
        public decimal ZwlBalance => TotalZwlIncome - TotalZwlExpense;

        public List<RecentTransactionViewModel> RecentTransactions { get; set; }
        public Dictionary<string, decimal> MonthlyIncome { get; set; } = new();
        public Dictionary<string, decimal> MonthlyExpense { get; set; } = new();
    }
}