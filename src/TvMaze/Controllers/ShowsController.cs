using MediatR;
using Microsoft.AspNetCore.Mvc;
using TvMaze.Contracts.Queries;
using TvMaze.Models;

namespace TvMaze.Controllers;

[ApiController]
[Route("[controller]")]
public class ShowsController : ControllerBase
{
    private readonly ISender _sender;

    public ShowsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PageResult<ShowModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetShows(int page = 1, int pageSize = 25)
    {
        var query = new GetShowsQuery
        {
            Page = page,
            PageSize = pageSize
        };

        var result = await _sender.Send(query, HttpContext.RequestAborted);

        return result.Status switch
        {
            GetShowsQueryResultStatus.Ok => Ok(result.Data),
            GetShowsQueryResultStatus.NotFound => NotFound(),
            GetShowsQueryResultStatus.BadRequest => BadRequest(result.Message),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
