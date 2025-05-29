using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoLooseCent.DbContexts;
using NoLooseCent.Models;

namespace NoLooseCent.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExpenseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Expense
        public async Task<IActionResult> Index()
        {
            var expenses = _context.Expenses.Include(e => e.Currency);
            return View(await expenses.ToListAsync());
        }

        // GET: Expense/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var expense = await _context.Expenses.Include(e => e.Currency).FirstOrDefaultAsync(e => e.Id == id);
            if (expense == null) return NotFound();
            return View(expense);
        }

        // GET: Expense/Create
        public IActionResult Create()
        {
            ViewBag.Currencies = _context.Currencies.ToList();
            return View();
        }

        // POST: Expense/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Expense expense)
        {
            // Prevent model binding validation error on navigation property
            ModelState.Remove("Currency");

            if (ModelState.IsValid)
            {
                _context.Add(expense);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Currencies = _context.Currencies.ToList();
            return View(expense);
        }


        // GET: Expense/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null) return NotFound();

            ViewBag.Currencies = _context.Currencies.ToList();
            return View(expense);
        }

        // POST: Expense/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Expense expense)
        {
            if (id != expense.Id)
                return NotFound();

            // Fix navigation property validation
            ModelState.Remove("Currency");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(expense);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Expenses.Any(e => e.Id == id))
                        return NotFound();

                    throw;
                }
            }

            ViewBag.Currencies = _context.Currencies.ToList();
            return View(expense);
        }


        // GET: Expense/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var expense = await _context.Expenses.Include(e => e.Currency).FirstOrDefaultAsync(e => e.Id == id);
            if (expense == null) return NotFound();
            return View(expense);
        }

        // POST: Expense/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}