using Microsoft.EntityFrameworkCore;
using TvMaze.Data;
using TvMaze.Models;

namespace TvMaze.ApplicationServices;

public class ShowProvider
{
    private readonly PaginationConfig _paginationConfig;
    private readonly TvMazeDbContext _context;

    public ShowProvider(PaginationConfig paginationConfig, TvMazeDbContext context)
    {
        _paginationConfig = paginationConfig;
        _context = context;
    }

    public async Task<PageResult<ShowModel>> GetShowAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        if (page <= 0) throw new ArgumentOutOfRangeException(nameof(page));
        if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize));
        
        pageSize = Math.Min(pageSize, _paginationConfig.MaximumPageSize);
        
        var query = _context.Shows.Select(x => new ShowModel
        {
            Id = x.Id,
            Name = x.Name,
            Cast = x.Cast.Select(c => new ActorModel
            {
                Id = c.Id,
                Name = c.Name,
                Birthday = c.DateOfBirth
            }).OrderBy(c => c.Birthday).ToList()
        }).OrderBy(x => x.Id);

        var count = await query.CountAsync(cancellationToken);
        var items = await query.Skip(pageSize * (page - 1)).Take(pageSize).ToListAsync(cancellationToken);

        return new PageResult<ShowModel>
        {
            Items = items,
            CurrentPage = page,
            TotalCount = count
        };
    }
}