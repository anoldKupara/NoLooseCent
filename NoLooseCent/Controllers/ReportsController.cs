using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoLooseCent.DbContexts;
using NoLooseCent.ViewModels;

namespace NoLooseCent.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
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
                TotalUsdIncome = await _context.Incomes.Where(i => usdCurrencyIds.Contains(i.CurrencyId)).SumAsync(i => (decimal?)i.Amount) ?? 0,
                TotalUsdExpense = await _context.Expenses.Where(e => usdCurrencyIds.Contains(e.CurrencyId)).SumAsync(e => (decimal?)e.Amount) ?? 0,
                TotalZwlIncome = await _context.Incomes.Where(i => zwlCurrencyIds.Contains(i.CurrencyId)).SumAsync(i => (decimal?)i.Amount) ?? 0,
                TotalZwlExpense = await _context.Expenses.Where(e => zwlCurrencyIds.Contains(e.CurrencyId)).SumAsync(e => (decimal?)e.Amount) ?? 0
            };

            return View(model);
        }
    }
}