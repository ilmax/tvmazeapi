namespace TvMaze.Domain;

public class JobRunMetadata
{
    // Used by EF core
    private JobRunMetadata() { }
    
    public JobRunMetadata(DateTimeOffset runAtUtc, RunStatus status, int lastSuccessfulShowPageFetched)
    {
        RunAtUtc = runAtUtc;
        Status = status;
        LastSuccessfulShowPageFetched = lastSuccessfulShowPageFetched;
    }

    private long Id { get; }
    public DateTimeOffset RunAtUtc { get; private set; }
    public RunStatus Status { get; private set; }
    public int LastSuccessfulShowPageFetched { get; private set; }
}

public enum RunStatus
{
    RateLimitOnShows,
    RateLimitOnCasts,
    RunSuccessful
}