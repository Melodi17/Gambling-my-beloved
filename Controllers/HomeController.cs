using System.Diagnostics;
using Gambling_my_beloved.Data;
using Microsoft.AspNetCore.Mvc;
using Gambling_my_beloved.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Gambling_my_beloved.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        this._logger = logger;
        this._context = context;
        this._userManager = userManager;
        this._signInManager = signInManager;
    }

    public IActionResult Index()
    {
        // Set ViewData["StockEvents"] to 10 last StockEvents

        // Assuming events already includes necessary data, we'll focus on optimizing the Companies query
        
        IQueryable<StockEvent> events = this._context.StockEvents
            .OrderByDescending(e => e.Date)
            .Take(5)
            .Include(e => e.Company)
            .ThenInclude(e => e.Stocks)
            .ThenInclude(s => s.PriceHistory);

        List<StockEvent> eventsList = events.ToList();
        IQueryable<Stock> stocks = this._context.Stocks
            .Include(s => s.Company)
            .Include(s => s.PriceHistory);
        
        if (this._signInManager.IsSignedIn(User))
        {
            string userId = this._userManager.GetUserId(User);
            ViewData["User"] = this._context.Users
                .Include(u => u.Stocks)
                .ThenInclude(s => s.Stock)
                .ThenInclude(s => s.PriceHistory)
                .Include(u => u.Stocks)
                .ThenInclude(s => s.Transactions)
                .FirstOrDefault(u => u.Id == userId);
        }
        else
            ViewData["User"] = null;

        ViewData["StockEvents"] = eventsList;
        ViewData["EffectedStocks"] = eventsList.Select(x => x.EffectedStocks
            .Select(id => stocks.First(stock => stock.Id == id)).ToArray()).ToList();

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}