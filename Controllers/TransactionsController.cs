using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Gambling_my_beloved.Data;
using Gambling_my_beloved.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Gambling_my_beloved.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private const int DefaultLimit = 30;
        private const int MaxLimit = 50;

        public TransactionsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Transactions
        public async Task<IActionResult> Index(int? offset, int? limit)
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            IQueryable<Transaction> applicationDbContext = _context.Transactions
                .Include(t => t.StockOwnership)
                .ThenInclude(t => t.Stock)
                .ThenInclude(t => t.Company)
                .Where(t => t.StockOwnership.AccountId == user.Id)
                .OrderByDescending(t => t.Date);
                // .Skip(offset ?? 0)
                // .Take(Math.Min(limit ?? DefaultLimit, MaxLimit));
            
            return View(await applicationDbContext.ToListAsync());
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.Id == id);
        }
    }
}
