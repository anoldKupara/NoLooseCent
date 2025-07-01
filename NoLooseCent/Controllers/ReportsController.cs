using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoLooseCent.DbContexts;
using NoLooseCent.Models;
using NoLooseCent.ViewModels;

namespace NoLooseCent.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReportsController> _log;

        public ReportsController(ApplicationDbContext context, ILogger<ReportsController> log)
        {
            _context = context;
            _log = log;
        }

        public async Task<IActionResult> Index()
        {
            var usdCurrencyIds = await _context.Currencies
                .Where(c => c.Code.StartsWith("USD"))
                .Select(c => c.Id)
                .ToListAsync();

            var zwlCurrencyIds = await _context.Currencies
                .Where(c => c.Code.StartsWith("ZWL"))
                .Select(c => c.Id)
                .ToListAsync();

            var model = new DashboardViewModel
            {
                TotalUsdIncome = await _context.Incomes
                    .Where(i => usdCurrencyIds.Contains(i.CurrencyId))
                    .SumAsync(i => (decimal?)i.Amount) ?? 0,

                TotalUsdExpense = await _context.Expenses
                    .Where(e => usdCurrencyIds.Contains(e.CurrencyId))
                    .SumAsync(e => (decimal?)e.Amount) ?? 0,

                TotalZwlIncome = await _context.Incomes
                    .Where(i => zwlCurrencyIds.Contains(i.CurrencyId))
                    .SumAsync(i => (decimal?)i.Amount) ?? 0,

                TotalZwlExpense = await _context.Expenses
                    .Where(e => zwlCurrencyIds.Contains(e.CurrencyId))
                    .SumAsync(e => (decimal?)e.Amount) ?? 0
            };

            return View(model);
        }

        public async Task<IActionResult> FinancialStatement(int currencyId = 1)
        {
            var viewModel = await BuildMonthlyStatementsAsync(currencyId);
            return View(viewName: "FinancialStatement", model: viewModel);
        }

        /* ────────── helper that builds the month-by-month statement ────────── */
        private async Task<List<MonthlyStatementViewModel>> BuildMonthlyStatementsAsync(
            int currencyId,
            DateTime startDate = default)
        {
            if (startDate == default)
                startDate = new DateTime(2025, 6, 1);          // first month you used the app

            /* 1.  pull rows */
            var incomes = await _context.Incomes
                .Where(i => i.CurrencyId == currencyId && i.DateReceived >= startDate)
                .Select(i => new
                {
                    Date = i.DateReceived,
                    Description = i.Source,
                    Amount = i.Amount            // positive
                })
                .ToListAsync();

            var expenses = await _context.Expenses
                .Where(e => e.CurrencyId == currencyId && e.DateSpent >= startDate)
                .Select(e => new
                {
                    Date = e.DateSpent,
                    Description = e.Purpose,
                    Amount = -e.Amount            // negative
                })
                .ToListAsync();

            /* 2. merge & sort */
            var tx = incomes.Concat(expenses)
                            .OrderBy(t => t.Date)
                            .ToList();

            /* 3. group into months */
            var groups = tx.GroupBy(t => new { t.Date.Year, t.Date.Month })
                           .OrderBy(g => g.Key.Year)
                           .ThenBy(g => g.Key.Month);

            /* 4. running balance rolled forward */
            decimal running = 0m;
            var currencyCode = await _context.Currencies
                                             .Where(c => c.Id == currencyId)
                                             .Select(c => c.Code)
                                             .FirstAsync();

            var result = new List<MonthlyStatementViewModel>();

            foreach (var g in groups)
            {
                var m = new MonthlyStatementViewModel
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    OpeningBalance = running,
                    CurrencyCode = currencyCode
                };

                foreach (var row in g)
                {
                    var deposit = row.Amount > 0 ? row.Amount : (decimal?)null;
                    var withdrawal = row.Amount < 0 ? -row.Amount : (decimal?)null;

                    running += row.Amount;

                    m.Entries.Add(new StatementEntry
                    {
                        Date = row.Date,
                        Description = row.Description,
                        Deposit = deposit,
                        Withdrawal = withdrawal,
                        Balance = running
                    });
                }

                m.ClosingBalance = running;
                result.Add(m);
            }

            return result;
        }
    }
}