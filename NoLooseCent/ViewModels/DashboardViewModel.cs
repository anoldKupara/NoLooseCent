namespace NoLooseCent.ViewModels
{
    public class DashboardViewModel
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Balance => TotalIncome - TotalExpense;

        public List<RecentTransactionViewModel> RecentTransactions { get; set; }
    }
}