namespace TvMaze.ApplicationServices;

public class ApiScraperBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly PeriodicTimer _timer;

    public ApiScraperBackgroundService(ScraperConfig scraperConfig, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(scraperConfig.IntervalInSeconds));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        do
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            await scope.ServiceProvider.GetRequiredService<ScraperService>().RunAsync(stoppingToken);
        } while (await _timer.WaitForNextTickAsync(stoppingToken));
    }

    public override void Dispose()
    {
        _timer?.Dispose();
        base.Dispose();
    }
}
