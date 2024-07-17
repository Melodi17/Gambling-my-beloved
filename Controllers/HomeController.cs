using System.Diagnostics;
using Gambling_my_beloved.Data;
using Microsoft.AspNetCore.Mvc;
using Gambling_my_beloved.Models;
using Gambling_my_beloved.Models.View;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Gambling_my_beloved.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context,
        UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        this._logger = logger;
        this._context = context;
        this._userManager = userManager;
        this._signInManager = signInManager;
    }

    public IActionResult Index()
    {
        IQueryable<StockEvent> events = this._context.StockEvents
            .OrderByDescending(e => e.Date)
            .Take(5);

        List<StockEvent> eventsList = events.ToList();

        // var events = this._context.StockEvents
        //     .OrderByDescending(e => e.Date)
        //     .Take(5)
        //     .Select(e => new
        //     {
        //         e.Id,
        //         e.Date,
        //         e.Company,
        //         EffectedStocks = e.EffectedStocks
        //             .Select(id => this._context.Stocks.FirstOrDefault(s => s.Id == id))
        //             .Select(s => new
        //             {
        //                 s.Id,
        //                 s.Symbol,
        //                 PriceHistory = s.PriceHistory
        //                     .OrderByDescending(p => p.Date)
        //                     .Take(2)
        //                     .Select(p => new
        //                     {
        //                         p.Date,
        //                         p.Price
        //                     }).ToList()
        //             }).ToList()
        //     }).ToList();

        IQueryable<Stock> stocks = this._context.Stocks
            .Include(s => s.Company)
            .Include(s => s.PriceHistory
                .OrderByDescending(p => p.Date)
                .Take(5));

        ApplicationUser? user = null;
        if (this._signInManager.IsSignedIn(User))
        {
            string userId = this._userManager.GetUserId(User);
            user = this._context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.Stocks)
                .ThenInclude(s => s.Stock)
                .ThenInclude(s => s.PriceHistory
                    .OrderByDescending(p => p.Date)
                    .Take(100)
                )
                .Include(u => u.Stocks)
                .ThenInclude(s => s.Transactions)
                .Include(u => u.Stocks)
                .ThenInclude(s => s.Stock)
                .ThenInclude(s => s.Company)
                .FirstOrDefault();

            List<(int quantity, Stock stock)> ownedStocks = user.GetOwnedStocks();
            ViewData["StockViewModels"] = ownedStocks
                .Select(x => new StockViewModel(x.stock, quantity: x.quantity, price: x.stock.UnitPrice,
                    priceChange: x.stock.PriceChange, pricePercent: x.stock.PriceChangePercent)).ToList();

            // ViewData["StockHistory"] = JsonConvert.SerializeObject(stock.PriceHistory
            //     .Select(p => new { Date = p.Date, Price = p.Price }));

            ViewData["StockHistory"] = JsonConvert.SerializeObject(ownedStocks.Select(s => new
            {
                Symbol = s.stock.Symbol,
                Color = s.stock.Color,
                History = s.stock.PriceHistory.Select(p => new
                    { Date = p.Date, Price = p.Price, PriceText = p.Price.ToCurrency() })
            }));
        }

        ViewData["Leaderboard"] = this._context.Users
            .Include(u => u.Stocks)
            .ThenInclude(u => u.Transactions)
            .Include(u => u.Stocks)
            .ThenInclude(u => u.Stock)
            .AsEnumerable()
            .Select(u => new UserViewModel
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                Balance = u.Balance,
                NetWorth = u.GetNetWorth(),
                Shares = u.GetOwnedStocks().Count
            })
            .OrderByDescending(u => u.NetWorth)
            .Take(5)
            .ToList();


        ViewData["User"] = user;

        ViewData["StockEvents"] = events;
        ViewData["EffectedStocks"] = eventsList.Select(x => x.EffectedStocks
            .Select(id => stocks.First(stock => stock.Id == id)).ToArray()).ToList();

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}