using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoLooseCent.DbContexts;
using NoLooseCent.ViewModels;

namespace NoLooseCent.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Assume: Currency.Code is "USD-*" or "ZWL"
            var usdCurrencyIds = await _context.Currencies
                .Where(c => c.Code.StartsWith("USD"))
                .Select(c => c.Id)
                .ToListAsync();

            var zwlCurrencyIds = await _context.Currencies
                .Where(c => c.Code.StartsWith("ZWL"))
                .Select(c => c.Id)
                .ToListAsync();

            var totalUsdIncome = await _context.Incomes
                .Where(i => usdCurrencyIds.Contains(i.CurrencyId))
                .SumAsync(i => (decimal?)i.Amount) ?? 0;

            var totalUsdExpense = await _context.Expenses
                .Where(e => usdCurrencyIds.Contains(e.CurrencyId))
                .SumAsync(e => (decimal?)e.Amount) ?? 0;

            var totalZwlIncome = await _context.Incomes
                .Where(i => zwlCurrencyIds.Contains(i.CurrencyId))
                .SumAsync(i => (decimal?)i.Amount) ?? 0;

            var totalZwlExpense = await _context.Expenses
                .Where(e => zwlCurrencyIds.Contains(e.CurrencyId))
                .SumAsync(e => (decimal?)e.Amount) ?? 0;

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
                TotalUsdIncome = totalUsdIncome,
                TotalUsdExpense = totalUsdExpense,
                TotalZwlIncome = totalZwlIncome,
                TotalZwlExpense = totalZwlExpense,
                RecentTransactions = allRecent
            };

            return View(model);
        }
    }
}