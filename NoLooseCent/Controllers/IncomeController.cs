using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoLooseCent.DbContexts;
using NoLooseCent.Models;

namespace NoLooseCent.Controllers
{
    public class IncomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IncomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var incomes = _context.Incomes.Include(i => i.Currency);
            return View(await incomes.ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var income = await _context.Incomes.Include(i => i.Currency).FirstOrDefaultAsync(i => i.Id == id);
            if (income == null) return NotFound();
            return View(income);
        }

        public IActionResult Create()
        {
            ViewBag.Currencies = _context.Currencies.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Income income)
        {
            if (ModelState.IsValid)
            {
                _context.Add(income);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Currencies = _context.Currencies.ToList();
            return View(income);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var income = await _context.Incomes.FindAsync(id);
            if (income == null) return NotFound();
            ViewBag.Currencies = _context.Currencies.ToList();
            return View(income);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Income income)
        {
            if (id != income.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(income);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Currencies = _context.Currencies.ToList();
            return View(income);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var income = await _context.Incomes.Include(i => i.Currency).FirstOrDefaultAsync(i => i.Id == id);
            if (income == null) return NotFound();
            return View(income);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var income = await _context.Incomes.FindAsync(id);
            _context.Incomes.Remove(income);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
