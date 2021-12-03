using MediatR;
using TvMaze.Models;

namespace TvMaze.Contracts.Queries;

public record GetShowsQuery : IRequest<GetShowsQueryResult>
{
    public int Page { get; init; }
    public int PageSize { get; init; }
}

public record GetShowsQueryResult
{
    public PageResult<ShowModel> Data { get; init; }
    public GetShowsQueryResultStatus Status { get; init; }
    public string Message { get; init; }
}

public enum GetShowsQueryResultStatus
{
    Ok,
    NotFound,
    BadRequest
}