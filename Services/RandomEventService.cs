using Gambling_my_beloved.Data;
using Gambling_my_beloved.Models;
using Microsoft.EntityFrameworkCore;

namespace Gambling_my_beloved.Services;

public class RandomEventService : IHostedService, IDisposable
{
    private readonly ILogger<RandomEventService> _logger;
    private readonly ApplicationDbContext _dbContext;
    private Timer? _timer = null;

    public RandomEventService(ILogger<RandomEventService> logger, IServiceProvider provider)
    {
        this._logger = logger;

        using IServiceScope scope = provider.CreateScope();
        this._dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Random Event Service running.");

        _timer = new Timer(DoWork, null, TimeSpan.Zero,
            this.GetRandomTimeSpan());

        return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        this._timer!.Change(this.GetRandomTimeSpan(), TimeSpan.Zero);

        StockEvent stockEvent = Global.Random.Next(0, 2) == 0 
            ? StockEvent.GenerateRandomEventForCompany(this._dbContext.Companies) 
            : StockEvent.GenerateRandomEventForIndustry();

        if (stockEvent.Industry != null)
        {
            // Get all companies in the industry
            IQueryable<Company> companies = this._dbContext.Companies
                .Where(c => c.Industries.Contains(stockEvent.Industry.Value));
            
            // Apply the event to all companies in the industry
            foreach (Company company in companies.Include(company => company.Stocks))
            foreach (Stock stock in company.Stocks)
            {
                decimal priceChange = stock.UnitPrice * stockEvent.Weight * (stockEvent.IsPositive ? 1 : -1);
                
                
                
                stock.UpdatePrice(stock.UnitPrice + priceChange);
            }
        }
        
        
        
        this._dbContext.StockEvents.Add(stockEvent);
        
        _logger.LogInformation("Random Event Service is working.");
    }
    
    private TimeSpan GetRandomTimeSpan()
    {
        return TimeSpan.FromSeconds(Global.Random.Next(3, 10));
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