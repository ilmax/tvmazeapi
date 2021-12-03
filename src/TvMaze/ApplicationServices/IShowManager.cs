using TvMaze.Domain;

namespace TvMaze.ApplicationServices;

public interface IShowManager
{
    Task<IEnumerable<long>> GetShowIdsWithoutCastAsync(CancellationToken cancellationToken);
    Task<JobRunMetadata> GetLastRunAsync(CancellationToken cancellationToken);
    Task StoreShowsPageAsync(JobRunMetadata jobRunMetadata, IEnumerable<Show> shows, int currentPage, CancellationToken cancellationToken);
    Task StoreCastForShowAsync(JobRunMetadata jobRunMetadata, long showId, IEnumerable<CastEntry> cast, CancellationToken cancellationToken);
    Task StoreLastRunMetadataAsync(int showPageId, RunStatus status, CancellationToken cancellationToken);
}