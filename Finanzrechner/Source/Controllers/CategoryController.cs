using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Finanzrechner.Database;
using Finanzrechner.Models;

namespace Finanzrechner.Controllers
{
    public class CategoryController : Controller
    {
        private readonly DatabaseContext _context;

        public CategoryController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: Category
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult SwitchTransactions(int categoryId)
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories.Where(x => x.Id != categoryId), "Id", "Name");
            return View(new SwitchTransactionsModel { Category = _context.Categories.Where(x => x.Id == categoryId).First(), FromCategoryId = 0, ToCategoryId = 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PostSwitchTransactions(SwitchTransactionsModel model)
        {
            List<Transaction> transactionsFromOldCategory = _context.Transactions.Where(x => x.CategoryId == model.FromCategoryId).ToList();

            foreach (Transaction transaction in transactionsFromOldCategory)
            {
                transaction.CategoryId = model.ToCategoryId;
            }

            _context.SaveChanges();

            _context.Categories.Remove(_context.Categories.Where(x => x.Id == model.FromCategoryId).First());

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // POST: Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ColorCode")] Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Category/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Category/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ColorCode")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            ViewBag.CanDelete = false;

            if (_context.Categories.Count() > 1)
            {
                ViewBag.CanDelete = true;
            }

            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            return RedirectToAction("SwitchTransactions", "Category", new { CategoryId = id });
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
