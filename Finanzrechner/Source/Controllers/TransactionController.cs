using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Finanzrechner.Database;
using Microsoft.IdentityModel.Tokens;

namespace Finanzrechner.Controllers
{
    public class TransactionController : Controller
    {
        private readonly DatabaseContext _context;

        public TransactionController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: Transaction
        public async Task<IActionResult> Index(int? amountEntriesShown, string? searchString, DateTime? dateFrom, DateTime? dateTo, bool? intake, int? category)
        {
            List<Transaction> transactions = _context.Transactions.Include(x => x.Category).OrderByDescending(x => x.TimeStamp).ToList();

            ViewBag.ShowDeleteFilterButton = false;

            List<Category> selectlistCategories =
            [
                new Category
                {
                    Id = -1,
                    Name = "Kategorie wählen",
                    ColorCode = "#000000"
                },
                .. _context.Categories
            ];

            if (category is not null)
            {
                if (category != -1)
                {
                    transactions = transactions.Where(x => x.CategoryId == category).ToList();
                    ViewData["CategoryId"] = new SelectList(selectlistCategories, "Id", "Name", _context.Categories.Where(x => x.Id == category).Select(x => x.Id).First());
                    ViewBag.ShowDeleteFilterButton = true;
                }
                else
                {
                    ViewData["CategoryId"] = new SelectList(selectlistCategories, "Id", "Name");
                }
            } else
            {
                ViewData["CategoryId"] = new SelectList(selectlistCategories, "Id", "Name");
            }

            if (intake is not null)
            {
                if ((bool)intake)
                {
                    ViewBag.IntakeFilter = true;
                    ViewBag.ShowDeleteFilterButton = true;
                    transactions = transactions.Where(x => x.IsIntake == true).ToList();
                }
                else
                {
                    ViewBag.IntakeFilter = false;
                    ViewBag.ShowDeleteFilterButton = true;
                    transactions = transactions.Where(x => x.IsIntake == false).ToList();
                }
            }

            // Date-Filter
            if (dateFrom is not null)
            {
                ViewBag.DateFromFilter = dateFrom.Value.ToString("yyyy-MM-dd");
                ViewBag.ShowDeleteFilterButton = true;
                transactions = transactions.Where(x => x.TimeStamp >= dateFrom).ToList();
            }

            if (dateTo is not null)
            {
                ViewBag.DateToFilter = dateTo.Value.ToString("yyyy-MM-dd");
                ViewBag.ShowDeleteFilterButton = true;
                transactions = transactions.Where(x => x.TimeStamp <= dateTo).ToList();
            }

            // Search-Filter
            if (!String.IsNullOrEmpty(searchString))
            {
                ViewBag.SearchFilter = searchString;
                ViewBag.ShowDeleteFilterButton = true;
                transactions = transactions.Where(x => x.Description is not null && x.Description.Contains(searchString)).ToList();
            }

            // Amount-Filter
            if (amountEntriesShown == 10)
            {
                ViewBag.SelectedAmountFilter = 1;
                ViewBag.ShowDeleteFilterButton = true;
                transactions = transactions.Take(10).ToList();
            } 
            else if (amountEntriesShown == 50)
            {
                ViewBag.SelectedAmountFilter = 2;
                ViewBag.ShowDeleteFilterButton = true;
                transactions = transactions.Take(50).ToList();
            }
            else
            {
                ViewBag.SelectedAmountFilter = 3;
            }

            return View(transactions);
        }

        // GET: Transaction/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Transaction/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Amount,TimeStamp,CategoryId,IsIntake")] Transaction transaction)
        {
            try
            {
                _context.Add(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", transaction.CategoryId);
                return View(transaction);
            }
        }

        // GET: Transaction/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", transaction.CategoryId);
            return View(transaction);
        }

        // POST: Transaction/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Amount,TimeStamp,CategoryId,IsIntake")] Transaction transaction)
        {
            if (id != transaction.Id)
            {
                return NotFound();
            }
                
            try
            {
                _context.Update(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", transaction.CategoryId);
                return View(transaction);
            }
        }

        // GET: Transaction/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.Id == id);
        }
    }
}
