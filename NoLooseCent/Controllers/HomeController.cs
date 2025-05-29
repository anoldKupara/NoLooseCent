using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoLooseCent.DbContexts;
using NoLooseCent.ViewModels;

namespace NoLooseCent.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var totalIncome = await _context.Incomes.SumAsync(i => (decimal?)i.Amount) ?? 0;
            var totalExpense = await _context.Expenses.SumAsync(e => (decimal?)e.Amount) ?? 0;

            var recentIncomes = await _context.Incomes
                .Include(i => i.Currency)
                .OrderByDescending(i => i.DateReceived)
                .Take(5)
                .Select(i => new RecentTransactionViewModel
                {
                    Type = "Income",
                    Amount = i.Amount,
                    SourceOrPurpose = i.Source,
                    Date = i.DateReceived,
                    CurrencyName = i.Currency.Name
                })
                .ToListAsync();

            var recentExpenses = await _context.Expenses
                .Include(e => e.Currency)
                .OrderByDescending(e => e.DateSpent)
                .Take(5)
                .Select(e => new RecentTransactionViewModel
                {
                    Type = "Expense",
                    Amount = e.Amount,
                    SourceOrPurpose = e.Purpose,
                    Date = e.DateSpent,
                    CurrencyName = e.Currency.Name
                })
                .ToListAsync();

            var allRecent = recentIncomes.Concat(recentExpenses)
                .OrderByDescending(t => t.Date)
                .Take(5)
                .ToList();

            var model = new DashboardViewModel
            {
                TotalIncome = totalIncome,
                TotalExpense = totalExpense,
                RecentTransactions = allRecent
            };

            return View(model);
        }
    }
}