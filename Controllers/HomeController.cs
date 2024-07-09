using System.Diagnostics;
using Gambling_my_beloved.Data;
using Microsoft.AspNetCore.Mvc;
using Gambling_my_beloved.Models;
using Microsoft.EntityFrameworkCore;

namespace Gambling_my_beloved.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        this._logger = logger;
        this._context = context;
    }

    public IActionResult Index()
    {
        // Set ViewData["StockEvents"] to 10 last StockEvents

        IQueryable<StockEvent> events = this._context.StockEvents
            .OrderByDescending(e => e.Date)
            .Take(30)
            .Include(e => e.Company)
            .ThenInclude(e => e.Stocks)
            .ThenInclude(s => s.PriceHistory);

        ViewData["StockEvents"] = events.ToList();
        // Each item in companies should be an array of either the targeted company or all companies that are part of the events industry
        ViewData["Companies"] = events.AsEnumerable().Select(e => e.Company != null
            ? new Company[] { e.Company }
            : this._context.Companies
                .Where(c => c.Industries.Contains(e.Industry.Value))
                .Include(c => c.Stocks)
                .ThenInclude(s => s.PriceHistory)
                .AsEnumerable()
                .OrderBy(c => c.Stocks.Sum(s => s.UnitPrice))
                .Take(3)
                .ToArray()).ToList();

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