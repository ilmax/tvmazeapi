namespace TvMaze.ApplicationServices;

public interface ITvMazeHttpService
{
    Task<ShowsApiServiceResult> GetShowsAsync(int page, CancellationToken cancellationToken);
    Task<CastApiServiceResult> GetCastAsync(long showId, CancellationToken cancellationToken);
}