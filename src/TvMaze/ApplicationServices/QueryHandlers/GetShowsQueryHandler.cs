using MediatR;
using TvMaze.Contracts.Queries;

namespace TvMaze.ApplicationServices.QueryHandlers;

public class GetShowsQueryHandler : IRequestHandler<GetShowsQuery, GetShowsQueryResult>
{
    private readonly ShowProvider _showProvider;

    public GetShowsQueryHandler(ShowProvider showProvider)
    {
        _showProvider = showProvider;
    }

    public async Task<GetShowsQueryResult> Handle(GetShowsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _showProvider.GetShowAsync(request.Page, request.PageSize, cancellationToken);
            return new GetShowsQueryResult
            {
                Data = result,
                Status = !result.Items.Any() ? GetShowsQueryResultStatus.NotFound : GetShowsQueryResultStatus.Ok
            };
        }
        catch (ArgumentOutOfRangeException e)
        {
            return new GetShowsQueryResult
            {
                Message = e.Message,
                Status = GetShowsQueryResultStatus.BadRequest
            };
        }
    }
}