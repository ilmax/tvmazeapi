using TvMaze.Domain;

namespace TvMaze.ApplicationServices;

public class ScraperService
{
    private readonly ITvMazeHttpService _tvMazeHttpService;
    private readonly IShowManager _showManager;
    private readonly ILogger<ScraperService> _logger;

    public ScraperService(ITvMazeHttpService tvMazeHttpService, IShowManager showManager, ILogger<ScraperService> logger)
    {
        _tvMazeHttpService = tvMazeHttpService ?? throw new ArgumentNullException(nameof(tvMazeHttpService));
        _showManager = showManager ?? throw new ArgumentNullException(nameof(showManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        var lastRun = await _showManager.GetLastRunAsync(cancellationToken);

        var runInfo = new RunInfo
        {
            RunStatus = lastRun?.Status ?? RunStatus.RunSuccessful,
            LastFetchedShowPage = lastRun?.LastSuccessfulShowPageFetched ?? 0
        };

        switch (runInfo.RunStatus)
        {
            case RunStatus.RateLimitOnShows:
                runInfo = await GetShowsAsync(runInfo, cancellationToken);
                break;
            case RunStatus.RateLimitOnCasts:
                var showIdsWithoutCast = await _showManager.GetShowIdsWithoutCastAsync(cancellationToken);
                runInfo = await GetShowCastAsync(runInfo, showIdsWithoutCast, cancellationToken);
                break;
            case RunStatus.RunSuccessful:
                runInfo = await GetShowsAsync(runInfo, cancellationToken);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(runInfo.RunStatus), $"Type {runInfo.RunStatus} is not expected");
        }

        await _showManager.StoreLastRunMetadataAsync(runInfo.LastFetchedShowPage, runInfo.RunStatus, cancellationToken);
    }

    private async Task<RunInfo> GetShowsAsync(RunInfo runInfo, CancellationToken cancellationToken)
    {
        var result = await _tvMazeHttpService.GetShowsAsync(runInfo.LastFetchedShowPage + 1, cancellationToken);

        switch (result)
        {
            case ShowsServiceResult showResult:
                runInfo = runInfo with { LastFetchedShowPage = runInfo.LastFetchedShowPage + 1 };
                await _showManager.StoreShowsPageAsync(FromRunInfo(runInfo), showResult.Shows, runInfo.LastFetchedShowPage, cancellationToken);
                runInfo = await GetShowCastAsync(runInfo, showResult.Shows.Select(x => x.Id), cancellationToken);
                break;
            case ShowNotFoundServiceResult:
                _logger.LogInformation("No more shows left on the tv maze api");
                runInfo = runInfo with { RunStatus = RunStatus.RunSuccessful };
                break;
            case TooManyShowsRequestServiceResult:
                _logger.LogWarning("Reached api rate limiting, going to sleep and coming back in a while");
                runInfo = runInfo with { RunStatus = RunStatus.RateLimitOnShows };
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(result), $"Type {result?.GetType().FullName ?? "null"} is not expected");
        }

        return runInfo;
    }

    private async Task<RunInfo> GetShowCastAsync(RunInfo runInfo, IEnumerable<long> showIds, CancellationToken cancellationToken)
    {
        foreach (var showId in showIds)
        {
            runInfo = await GetCastForShowAsync(runInfo, showId, cancellationToken);
            if (runInfo.RunStatus != RunStatus.RunSuccessful)
            {
                return runInfo;
            }
        }

        return runInfo;
    }

    private async Task<RunInfo> GetCastForShowAsync(RunInfo runInfo, long showId, CancellationToken cancellationToken)
    {
        var result = await _tvMazeHttpService.GetCastAsync(showId, cancellationToken);

        switch (result)
        {
            case CastServiceResult cast:
                await _showManager.StoreCastForShowAsync(FromRunInfo(runInfo), cast.ShowId, cast.Cast, cancellationToken);
                runInfo = runInfo with { RunStatus = RunStatus.RunSuccessful };
                break;
            case CastNotFoundServiceResult:
                _logger.LogInformation($"No cast found for show with id {showId}");
                await _showManager.StoreCastForShowAsync(FromRunInfo(runInfo), showId, Enumerable.Empty<CastEntry>(), cancellationToken);
                runInfo = runInfo with { RunStatus = RunStatus.RunSuccessful };
                break;
            case TooManyCastsRequestServiceResult:
                _logger.LogWarning("Reached api rate limiting, going to sleep and coming back in a while");
                runInfo = runInfo with { RunStatus = RunStatus.RateLimitOnCasts };
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(result), $"Type {result?.GetType().FullName ?? "null"} is not expected");
        }

        return runInfo;
    }

    private static JobRunMetadata FromRunInfo(RunInfo runInfo) => 
        new(DateTimeOffset.UtcNow, runInfo.RunStatus, runInfo.LastFetchedShowPage);

    private record RunInfo
    {
        public int LastFetchedShowPage { get; init; }
        public RunStatus RunStatus { get; init; }
    }
}