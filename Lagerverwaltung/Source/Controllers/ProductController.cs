using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lagerverwaltung.Database;
using Microsoft.Identity.Client.Extensions.Msal;

namespace Lagerverwaltung.Controllers
{
    public class ProductController : Controller
    {
        private readonly DatabaseContext _context;

        public ProductController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: Product
        public async Task<IActionResult> Index(string? searchString, int? inStorageFilter, int? category)
        {
            List<Product> products = _context.Products.Include(x => x.Category).ToList();

            ViewBag.ShowDeleteFilterButton = false;

            List<Category> selectlistCategories =
            [
                new Category
                {
                    Id = -1,
                    Name = "-- Kategorie wählen --"
                },
                .. _context.Categories
            ];

            if (category is not null)
            {
                if (category != -1)
                {
                    products = products.Where(x => x.CategoryId == category).ToList();
                    ViewData["CategoryId"] = new SelectList(selectlistCategories, "Id", "Name", _context.Categories.Where(x => x.Id == category).Select(x => x.Id).First());
                    ViewBag.ShowDeleteFilterButton = true;
                }
                else
                {
                    ViewData["CategoryId"] = new SelectList(selectlistCategories, "Id", "Name");
                }
            }
            else
            {
                ViewData["CategoryId"] = new SelectList(selectlistCategories, "Id", "Name");
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                ViewBag.SearchFilter = searchString;
                ViewBag.ShowDeleteFilterButton = true;
                products = products.Where(x => x.Name.Contains(searchString)).ToList();
            }

            if (inStorageFilter is not null)
            {
                ViewBag.InStorage = inStorageFilter;
                ViewBag.ShowDeleteFilterButton = true;
                products = products.Where(x => x.InStorage == (int)inStorageFilter).ToList();
            }

            return View(products);
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(x => x.Sales)
                .Include(x => x.Reorders)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            product.Sales = product.Sales.OrderByDescending(x => x.SaleDate).Take(10).ToList();

            return View(product);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Set<Category>(), "Id", "Name");
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,InStorage,CurrentSellPrice,CategoryId")] Product product)
        {
            try{
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }catch (Exception e)
            {

            }
            ViewData["CategoryId"] = new SelectList(_context.Set<Category>(), "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Set<Category>(), "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,InStorage,CurrentSellPrice,CategoryId")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Set<Category>(), "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
