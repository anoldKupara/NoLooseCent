using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoLooseCent.DbContexts;

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
            var balance = totalIncome - totalExpense;

            ViewBag.TotalIncome = totalIncome;
            ViewBag.TotalExpense = totalExpense;
            ViewBag.Balance = balance;

            return View();
        }
    }
}
