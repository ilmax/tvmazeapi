using Microsoft.EntityFrameworkCore;
using TvMaze.Data;
using TvMaze.Domain;

namespace TvMaze.ApplicationServices;

public class ShowManager : IShowManager
{
    private readonly TvMazeDbContext _context;

    public ShowManager(TvMazeDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<long>> GetShowIdsWithoutCastAsync(CancellationToken cancellationToken)
    {
        return await _context.Shows.Where(x => !x.HasNoCastInTheApi && !x.Cast.Any())
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<JobRunMetadata> GetLastRunAsync(CancellationToken cancellationToken)
    {
        return await _context.JobRunMetadata
            .OrderByDescending(x => x.RunAtUtc)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task StoreShowsPageAsync(JobRunMetadata jobRunMetadata, IEnumerable<Show> shows, int currentPage, CancellationToken cancellationToken)
    {
        _context.Shows.AddRange(shows.Select(x => new Domain.Show(x.Id, x.Name, currentPage)));
        _context.JobRunMetadata.Add(jobRunMetadata);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task StoreCastForShowAsync(JobRunMetadata jobRunMetadata, long showId, IEnumerable<CastEntry> cast, CancellationToken cancellationToken)
    {
        var show = await _context.Shows.SingleOrDefaultAsync(x => x.Id == showId, cancellationToken);
        if (show is null)
        {
            throw new InvalidOperationException($"Unable to find show with Id {showId}");
        }

        var actorIds = cast.Select(x => x.Person.Id).ToList();
        
        // Read the actor we have already persisted for this show to not generate duplicate actors
        var actorFromDb = await _context.Set<Actor>().Where(a => actorIds.Contains(a.Id)).ToListAsync(cancellationToken);
        var showCast = cast.DistinctBy(x => x.Person.Id)
            .Where(x => !actorFromDb.Any(a => a.Id == x.Person.Id))
            .Select(x => new Actor(x.Person.Id, x.Person.Name, x.Person.Birthday))
            .Concat(actorFromDb); 
        
        show.SetCast(showCast);
        _context.JobRunMetadata.Add(jobRunMetadata);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task StoreLastRunMetadataAsync(int showPageId, RunStatus status, CancellationToken cancellationToken)
    {
        var jobRunMetadata = new JobRunMetadata(DateTimeOffset.UtcNow, status, showPageId);
        _context.JobRunMetadata.Add(jobRunMetadata);
        await _context.SaveChangesAsync(cancellationToken);
    }
}