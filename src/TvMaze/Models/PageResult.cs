namespace TvMaze.Models;

public record PageResult<T>
{
    public IEnumerable<T> Items { get; init; }
    public long TotalCount { get; init; }
    public int CurrentPage { get; init; }
}