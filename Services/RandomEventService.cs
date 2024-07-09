using Gambling_my_beloved.Data;
using Gambling_my_beloved.Models;
using Microsoft.EntityFrameworkCore;

namespace Gambling_my_beloved.Services;

public class RandomEventService : IHostedService, IDisposable
{
    private readonly ILogger<RandomEventService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private Timer? _timer = null;

    public RandomEventService(ILogger<RandomEventService> logger, IServiceScopeFactory scopeFactory)
    {
        this._logger = logger;

        this._scopeFactory = scopeFactory;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Random Event Service running.");

        _timer = new Timer(DoWork, null, TimeSpan.Zero,
            this.GetRandomTimeSpan());

        return Task.CompletedTask;
    }
    
    private void UpdatePrice(Stock stock, StockEvent stockEvent)
    {
        decimal initialPrice = stock.UnitPrice;
        decimal priceChange = stock.UnitPrice * stockEvent.Weight * (stockEvent.IsPositive ? 1 : -1);
        // dial it down a bit
        priceChange /= 10;
                
        stock.UpdatePrice(stock.UnitPrice + priceChange);
        
        this._logger.Log(LogLevel.Information, 
            $"Stock {stock.Symbol} price changed from {initialPrice} to {stock.UnitPrice} due to event \"{stockEvent.Description}\"");
    }

    private TimeSpan GetRandomTimeSpan()
    {
        return TimeSpan.FromMinutes(Global.Random.Next(3, 10));
    }

    private void DoWork(object? state)
    {
        using IServiceScope scope = this._scopeFactory.CreateScope();
        ApplicationDbContext _dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        this._timer!.Change(this.GetRandomTimeSpan(), TimeSpan.Zero);

        StockEvent stockEvent = Global.Random.Next(0, 5) == 0 
            ? StockEvent.GenerateRandomEventForIndustry() 
            : StockEvent.GenerateRandomEventForCompany(_dbContext.Companies);

        if (stockEvent.Industry != null)
        {
            // Get all companies in the industry
            IQueryable<Company> companies = _dbContext.Companies
                .Where(c => c.Industries.Contains(stockEvent.Industry.Value));
            
            // Apply the event to all companies in the industry
            foreach (Company company in companies.Include(company => company.Stocks).ThenInclude(stock => stock.PriceHistory))
            foreach (Stock stock in company.Stocks)
                UpdatePrice(stock, stockEvent);
        }
        else if (stockEvent.Company != null)
        {
            // Apply the event to the company
            Company company = _dbContext.Companies
                .Include(c => c.Stocks)
                .ThenInclude(s => s.PriceHistory)
                .First(c => c.Id == stockEvent.Company.Id);
            
            foreach (Stock stock in company.Stocks)
                UpdatePrice(stock, stockEvent);
        }
        
        _dbContext.StockEvents.Add(stockEvent);

        _dbContext.SaveChanges();
        
        _logger.LogInformation("Random Event Service is working.");
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Random Event Service is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}