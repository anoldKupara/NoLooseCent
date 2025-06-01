namespace NoLooseCent.ViewModels
{
    public class DashboardViewModel
    {
        // Income/Expense Totals
        public decimal TotalUsdIncome { get; set; }
        public decimal TotalUsdExpense { get; set; }
        public decimal TotalZwlIncome { get; set; }
        public decimal TotalZwlExpense { get; set; }
        public decimal UsdBalance => TotalUsdIncome - TotalUsdExpense;
        public decimal ZwlBalance => TotalZwlIncome - TotalZwlExpense;

        public decimal ThisMonthIncome { get; set; }
        public decimal ThisMonthExpense { get; set; }
        public decimal ThisMonthBalance => ThisMonthIncome - ThisMonthExpense;


        // Savings Rate
        public decimal SavingsRate =>
            ThisMonthIncome == 0 ? 0 : Math.Round((ThisMonthBalance / ThisMonthIncome) * 100, 2);

        // Top Spending Categories
        public List<CategorySpendingViewModel> TopSpendingCategories { get; set; } = new();

        // Upcoming Events (planned future expenses or renewals)
        public List<UpcomingEventViewModel> UpcomingEvents { get; set; } = new();

        // Transactions
        public List<RecentTransactionViewModel> RecentTransactions { get; set; } = new();
        public List<CategorySpendingViewModel> TopCategories { get; internal set; }
    }

    public class CategorySpendingViewModel
    {
        public string Category { get; set; } = "";
        public decimal Amount { get; set; }
    }

    public class UpcomingEventViewModel
    {
        public string Title { get; set; } = "";
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
    }
}