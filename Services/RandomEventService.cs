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
    
    private void UpdatePrice(Stock stock, StockEvent stockEvent, DateTime time)
    {
        decimal initialPrice = stock.UnitPrice;
        decimal priceChange = stock.UnitPrice * stockEvent.Weight;
        
        // factor in the stock's volatility, which ranges between 0 and 10
        priceChange *= stock.Volatility / 10;
                
        stock.UpdatePrice(stock.UnitPrice + priceChange, time);
            
        this._logger.Log(LogLevel.Information, 
            $"Stock {stock.Symbol} price changed from {initialPrice} to {stock.UnitPrice} due to event \"{stockEvent.Description}\"");
    }

    private TimeSpan GetRandomTimeSpan()
    {
        // return TimeSpan.FromSeconds(1);
        return TimeSpan.FromSeconds(Global.Random.Next(10, 30));
        // return TimeSpan.FromMinutes(Global.Random.Next(3, 10));
    }

    private void DoWork(object? state)
    {
        using IServiceScope scope = this._scopeFactory.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        this._timer!.Change(this.GetRandomTimeSpan(), TimeSpan.Zero);

        StockEvent stockEvent = Global.Random.Next(0, 5) == 0 
            ? StockEvent.GenerateRandomEventForIndustry(dbContext.Companies)
            : StockEvent.GenerateRandomEventForCompany(dbContext.Companies);
        
        IQueryable<Stock> stocks = dbContext.Stocks
            .Include(stock => stock.PriceHistory);

        foreach (Stock stock in stockEvent.EffectedStocks.Select(id => stocks.First(stock => stock.Id == id)))
            UpdatePrice(stock, stockEvent, stockEvent.Date);
        
        dbContext.StockEvents.Add(stockEvent);

        dbContext.SaveChanges();
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