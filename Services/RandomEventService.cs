using Gambling_my_beloved.Data;
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

        this._dbContext.
        
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