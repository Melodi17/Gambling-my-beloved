using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Gambling_my_beloved.Data;
using Gambling_my_beloved.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace Gambling_my_beloved.Controllers
{
    public class StocksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StocksController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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

            return View(await stocks.Take(50).ToListAsync());
        }

        // GET: Stocks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            Stock stock = await _context.Stocks
                .Include(s => s.Company)
                .Include(s => s.PriceHistory)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (stock == null)
                return NotFound();
            
            string userId = _userManager.GetUserId(User);
            
            StockOwnership stockOwnership = _context.StockOwnerships
                .Include(s => s.Transactions)
                .Include(s=> s.Stock)
                .FirstOrDefault(s => s.StockId == stock.Id && s.AccountId == userId);
            
            ViewData["StockOwnership"] = stockOwnership;
            
            ViewData["StockHistory"] = JsonConvert.SerializeObject(stock.PriceHistory
                .Select(p => new { Date = p.Date, Price = p.Price }));

            return View(stock);
        }

        // GET: Stocks/Create
        public IActionResult Create()
        {
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name");
            return View();
        }

        // POST: Stocks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Symbol,CompanyId,UnitPrice,Volatility,Color")] Stock stock)
        {
            if (ModelState.IsValid)
            {
                _context.Add(stock);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", stock.CompanyId);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Symbol,CompanyId,UnitPrice,Volatility,Color")] Stock stock)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Transact(int stockId, TransactionType type, int quantity)
        {
            Stock stock = await _context.Stocks
                .Where(s => s.Id == stockId)
                .Include(s => s.PriceHistory)
                .FirstOrDefaultAsync();

            if (stock == null)
                return NotFound();
            
            string userId = _userManager.GetUserId(User);

            StockOwnership stockOwnership = this._context.StockOwnerships
                .Include(stockOwnership => stockOwnership.Account)
                .Include(stockOwnership => stockOwnership.Transactions)
                .FirstOrDefault(s => s.StockId == stock.Id && s.AccountId == userId);
            
            if (stockOwnership == null)
            {
                stockOwnership = new StockOwnership
                {
                    StockId = stock.Id,
                    AccountId = userId,
                    Transactions = new()
                };
                _context.StockOwnerships.Add(stockOwnership);
                
                await _context.SaveChangesAsync();
                
                stockOwnership = this._context.StockOwnerships
                    .Include(s => s.Account)
                    .Include(s => s.Transactions)
                    .FirstOrDefault(s => s.StockId == stock.Id && s.AccountId == userId);
            }
            
            decimal totalCost = quantity * stock.UnitPrice;
            
            Transaction transaction = new()
            {
                StockOwnershipId = stockOwnership.Id,
                StockOwnership = stockOwnership,
                Type = type,
                Quantity = quantity,
                Amount = totalCost,
                Date = DateTime.Now
            };
            
            if (type == TransactionType.Buy)
            {
                if (totalCost > stockOwnership.Account.Balance)
                {
                    ViewData["Success"] = false;
                    ViewData["Message"] = "Insufficient funds";
                    return this.View(transaction);
                }
                
                stockOwnership.Account.Balance -= totalCost;
            }
            else
            {
                if (quantity > stockOwnership.GetQuantity())
                {
                    ViewData["Success"] = false;
                    ViewData["Message"] = "Insufficient stocks";
                    return this.View(transaction);
                }
                
                stockOwnership.Account.Balance += totalCost;
            }
            
            _context.Transactions.Add(transaction);
            
            await _context.SaveChangesAsync();
            
            ViewData["Success"] = true;
            ViewData["Message"] = $"Transaction of {transaction.Amount.ToCurrency()} completed successfully";
            return View(transaction);
        }

        private bool StockExists(int id)
        {
            return _context.Stocks.Any(e => e.Id == id);
        }
    }
}