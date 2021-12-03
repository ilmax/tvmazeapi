namespace TvMaze;

public record PaginationConfig
{
    public int MaximumPageSize { get; init; } = 25;
}