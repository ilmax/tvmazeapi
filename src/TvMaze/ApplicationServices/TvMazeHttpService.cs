using System.Diagnostics;
using System.Net;
using System.Text.Json;

namespace TvMaze.ApplicationServices;

public class TvMazeHttpService : ITvMazeHttpService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _serializerOptions;

    public TvMazeHttpService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _serializerOptions = new JsonSerializerOptions();
        // _serializerOptions.AddContext<ShowContext>();
        _serializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    }

    public async Task<ShowsApiServiceResult> GetShowsAsync(int page, CancellationToken cancellationToken)
    {
        if (page <= 0) throw new ArgumentOutOfRangeException(nameof(page));

        var response = await _httpClient.GetAsync($"shows?page={page}", cancellationToken);
        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
                var shows = await response.Content.ReadFromJsonAsync<List<Show>>(_serializerOptions, cancellationToken);
                return new ShowsServiceResult { Shows = shows ?? Enumerable.Empty<Show>()};
            case HttpStatusCode.NotFound:
                return new ShowNotFoundServiceResult();
            case HttpStatusCode.TooManyRequests:
                return new TooManyShowsRequestServiceResult();
        }

        response.EnsureSuccessStatusCode();
        throw null; //never reached because the prev method will always throw exception, but we have to make the compiler happy
    }

    public async Task<CastApiServiceResult> GetCastAsync(long showId, CancellationToken cancellationToken)
    {
        if (showId <= 0) throw new ArgumentOutOfRangeException(nameof(showId));

        var response = await _httpClient.GetAsync($"shows/{showId}/cast", cancellationToken);

        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
                var cast = await response.Content.ReadFromJsonAsync<List<CastEntry>>(_serializerOptions, cancellationToken);
                return new CastServiceResult { Cast = cast, ShowId = showId };
            case HttpStatusCode.NotFound:
                return new CastNotFoundServiceResult();
            case HttpStatusCode.TooManyRequests:
                return new TooManyCastsRequestServiceResult();
        }

        response.EnsureSuccessStatusCode();
        throw null; //never reached because the prev method will always throw exception, but we have to make the compiler happy
    }
}

public abstract class CastApiServiceResult
{
}

public class CastServiceResult : CastApiServiceResult
{
    public long ShowId { get; init; }
    public IEnumerable<CastEntry> Cast { get; init; }
}

public class CastNotFoundServiceResult : CastApiServiceResult
{
}

public class TooManyCastsRequestServiceResult : CastApiServiceResult
{
}

public abstract class ShowsApiServiceResult
{
}

public class ShowsServiceResult : ShowsApiServiceResult
{
    public IEnumerable<Show> Shows { get; init; }
}

public class ShowNotFoundServiceResult : ShowsApiServiceResult
{
}

public class TooManyShowsRequestServiceResult : ShowsApiServiceResult
{
}