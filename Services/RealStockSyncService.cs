using Gambling_my_beloved.Data;
using Gambling_my_beloved.Models;
using Microsoft.EntityFrameworkCore;

namespace Gambling_my_beloved.Services;

public class RealStockSyncService : IHostedService, IDisposable
{
    private readonly ILogger<RandomEventService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private Timer? _timer = null;

    private Dictionary<int, decimal> _stockPrices = new();

    public RealStockSyncService(ILogger<RandomEventService> logger, IServiceScopeFactory scopeFactory)
    {
        this._logger = logger;

        this._scopeFactory = scopeFactory;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Real Stock Sync Service running.");

        _timer = new Timer(DoWork, null, TimeSpan.Zero,
            TimeSpan.FromMinutes(1));

        return Task.CompletedTask;
    }

    private void UpdatePrice(Stock stock, StockEvent stockEvent, DateTime time)
    {
        decimal initialPrice = stock.UnitPrice;
        decimal priceChange = stock.UnitPrice * stockEvent.Weight * (stockEvent.IsPositive ? 1 : -1);

        // factor in the stock's volatility, which ranges between 0 and 10
        priceChange *= stock.Volatility / 10;

        stock.UpdatePrice(stock.UnitPrice + priceChange, time);

        this._logger.Log(LogLevel.Information,
            $"Stock {stock.Symbol} price changed from {initialPrice} to {stock.UnitPrice} due to event \"{stockEvent.Description}\"");
    }

    private void DoWork(object? state)
    {
        using IServiceScope scope = this._scopeFactory.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        foreach (StockBinding stockBinding in dbContext.StockBindings
                     .Where(binding => binding.Type == BindingType.RealStock)
                     .Include(x => x.Stock)
                     .ThenInclude(x => x.Company)
                     .Include(x=>x.Stock)
                     .ThenInclude(x=>x.PriceHistory))
        {
            Stock stock = stockBinding.Stock;
            
            string[] stockInfo = stockBinding.BindTarget.Split(':');
            string stockSymbol = stockInfo[0];
            string stockExchange = stockInfo[1];
            decimal price = Utils.GetRealStockPrice(stockSymbol, stockExchange);
            
            if (!_stockPrices.ContainsKey(stock.Id))
                _stockPrices.Add(stock.Id, price);
            else if (_stockPrices[stock.Id] == price)
                continue;

            _stockPrices[stock.Id] = price;

            decimal initialPrice = stock.UnitPrice;
            decimal newPrice = price * stockBinding.Multiplier;

            if (initialPrice == newPrice)
                continue;

            StockEvent stockEvent = StockEvent.GenerateRandomTargetedEventForStock(stock, initialPrice < newPrice);
            stock.UpdatePrice(price * stockBinding.Multiplier, stockEvent.Date);

            dbContext.StockEvents.Add(stockEvent);

            _logger.LogInformation(
                $"Stock {stock.Symbol} price changed from {initialPrice} to {stock.UnitPrice} due to real stock price change");
        }

        dbContext.SaveChanges();
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Real Stock Sync Service is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}