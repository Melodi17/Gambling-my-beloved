using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Gambling_my_beloved.Data;
using Gambling_my_beloved.Models;

namespace Gambling_my_beloved.Controllers
{
    public class StocksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StocksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Stocks
        public async Task<IActionResult> Index(string searchQuery)
        {
            IQueryable<Stock> stocks = this._context.Stocks
                .Include(s => s.Company)
                .Include(s => s.PriceHistory)
                .OrderBy(s => s.Symbol)
                .Select(x => x);

            if (!string.IsNullOrEmpty(searchQuery))
                stocks = stocks.Where(s => EF.Functions.Like(s.Symbol, $"%{searchQuery}%"));
            
            ViewData["SearchQuery"] = searchQuery ?? string.Empty;

            return View(await stocks.ToListAsync());
        }

        // GET: Stocks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            Stock stock = await _context.Stocks
                .Include(s => s.Company)
                .Include(s => s.PriceHistory)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (stock == null)
                return NotFound();

            return View(stock);
        }

        // GET: Stocks/Create
        public IActionResult Create()
        {
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Id");
            return View();
        }

        // POST: Stocks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Symbol,CompanyId,UnitPrice")] Stock stock)
        {
            if (ModelState.IsValid)
            {
                _context.Add(stock);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Id", stock.CompanyId);
            return View(stock);
        }

        // GET: Stocks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stock = await _context.Stocks.FindAsync(id);
            if (stock == null)
            {
                return NotFound();
            }

            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Id", stock.CompanyId);
            return View(stock);
        }

        // POST: Stocks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Symbol,CompanyId,UnitPrice")] Stock stock)
        {
            if (id != stock.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stock);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockExists(stock.Id))
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

            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Id", stock.CompanyId);
            return View(stock);
        }

        // GET: Stocks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stock = await _context.Stocks
                .Include(s => s.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stock == null)
            {
                return NotFound();
            }

            return View(stock);
        }

        // POST: Stocks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stock = await _context.Stocks.FindAsync(id);
            if (stock != null)
            {
                _context.Stocks.Remove(stock);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StockExists(int id)
        {
            return _context.Stocks.Any(e => e.Id == id);
        }

        public static string GetStockChangeHtml(Stock stock)
        {
            decimal change = stock.GetPriceChange();
            string color = change > 0 ? "green" : change < 0 ? "red" : "black";
            return $"<span style='color: {color}'>{change}%</span>";
        }
    }
}