using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NoLooseCent.DbContexts;
using NoLooseCent.Models;
using NoLooseCent.Reports;
using NoLooseCent.ViewModels;       
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<IActionResult> FinancialStatement(
                string group = "USD",
                int? year = null,
                int? month = null)
        {
            var usdIds = await _context.Currencies
                          .Where(c => c.Code.StartsWith("USD"))
                          .Select(c => c.Id)
                          .ToListAsync();

            var zwlIds = await _context.Currencies
                          .Where(c => !c.Code.StartsWith("USD"))
                          .Select(c => c.Id)
                          .ToListAsync();

            var currencyGroups = new Dictionary<string, List<int>>
            {
                { "USD", usdIds },
                { "ZWL", zwlIds }
            };

            if (!currencyGroups.ContainsKey(group))
                group = "USD";

            var currencyIds = currencyGroups[group];

            if (year is null || month is null)
            {
                var latestTx = await _context.Incomes
                                 .Where(i => currencyIds.Contains(i.CurrencyId))
                                 .Select(i => (DateTime?)i.DateReceived)
                                 .Concat(_context.Expenses
                                     .Where(e => currencyIds.Contains(e.CurrencyId))
                                     .Select(e => (DateTime?)e.DateSpent))
                                 .OrderByDescending(d => d)
                                 .FirstOrDefaultAsync();

                if (latestTx is null)
                {
                    latestTx = DateTime.Today;
                }

                year ??= latestTx.Value.Year;
                month ??= latestTx.Value.Month;
            }

            var firstOfMonth = new DateTime(year.Value, month.Value, 1);

            var statement = await BuildSingleMonthStatementAsync(
                                currencyIds,
                                label: group,
                                firstOfMonth);

            ViewBag.CurrencyList = new SelectList(
                new[]
                {
                    new { Value = "USD", Text = "USD" },
                    new { Value = "ZWL", Text = "ZWL" }
                },
                dataValueField: "Value",
                dataTextField: "Text",
                selectedValue: group);

            ViewBag.MonthList = new SelectList(
                Enumerable.Range(1, 12)
                          .Select(m => new
                          {
                              Value = m,
                              Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m)
                          }),
                "Value",
                "Text",
                month);

            ViewBag.SelectedYear = year;

            return View("FinancialStatement", statement);
        }

        private async Task<MonthlyStatementViewModel> BuildSingleMonthStatementAsync(
                IReadOnlyCollection<int> currencyIds,
                string label,
                DateTime monthStart)
        {
            var monthEnd = monthStart.AddMonths(1);

            var totalIncomeBefore = await _context.Incomes
                .Where(i => currencyIds.Contains(i.CurrencyId) &&
                            i.DateReceived < monthStart)
                .SumAsync(i => (decimal?)i.Amount) ?? 0;

            var totalExpenseBefore = await _context.Expenses
                .Where(e => currencyIds.Contains(e.CurrencyId) &&
                            e.DateSpent < monthStart)
                .SumAsync(e => (decimal?)e.Amount) ?? 0;

            decimal running = totalIncomeBefore - totalExpenseBefore;

            var incomes = await _context.Incomes
                .Where(i => currencyIds.Contains(i.CurrencyId) &&
                            i.DateReceived >= monthStart &&
                            i.DateReceived < monthEnd)
                .Select(i => new
                {
                    Date = i.DateReceived,
                    Description = i.Source,
                    Amount = i.Amount      
                })
                .ToListAsync();

            var expenses = await _context.Expenses
                .Where(e => currencyIds.Contains(e.CurrencyId) &&
                            e.DateSpent >= monthStart &&
                            e.DateSpent < monthEnd)
                .Select(e => new
                {
                    Date = e.DateSpent,
                    Description = e.Purpose,
                    Amount = -e.Amount  
                })
                .ToListAsync();

            var tx = incomes.Concat(expenses)
                            .OrderBy(t => t.Date)
                            .ToList();

            var vm = new MonthlyStatementViewModel
            {
                Year = monthStart.Year,
                Month = monthStart.Month,
                OpeningBalance = running,
                CurrencyCode = label
            };

            foreach (var row in tx)
            {
                var deposit = row.Amount > 0 ? row.Amount : (decimal?)null;
                var withdrawal = row.Amount < 0 ? -row.Amount : (decimal?)null;

                running += row.Amount;

                vm.Entries.Add(new StatementEntry
                {
                    Date = row.Date,
                    Description = row.Description,
                    Deposit = deposit,
                    Withdrawal = withdrawal,
                    Balance = running
                });
            }

            vm.ClosingBalance = running;
            return vm;
        }
    }
}