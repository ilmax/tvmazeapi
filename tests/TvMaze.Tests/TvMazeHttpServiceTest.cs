using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TvMaze.ApplicationServices;
using Xunit;

namespace TvMaze.Tests;

public class TvMazeHttpServiceTest
{
    [Fact]
    public void Ctor_should_check_for_null()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TvMazeHttpService(null));
    }

    [Fact]
    public async Task GetShows_ok_returns_ok_response()
    {
        // Arrange
        using var handler = new MockHttpMessageHandler(HttpStatusCode.OK);
        using var client = new HttpClient(handler) { BaseAddress = new Uri("https://localhost") };
        var sut = new TvMazeHttpService(client);

        // Act
        var result = await sut.GetShowsAsync(1, default);

        // Assert
        Assert.IsType<ShowsServiceResult>(result);
    }

    [Fact]
    public async Task GetShows_not_found_returns_not_found_response()
    {
        // Arrange
        using var handler = new MockHttpMessageHandler(HttpStatusCode.NotFound);
        using var client = new HttpClient(handler) { BaseAddress = new Uri("https://localhost") };
        var sut = new TvMazeHttpService(client);

        // Act
        var result = await sut.GetShowsAsync(1, default);

        // Assert
        Assert.IsType<ShowNotFoundServiceResult>(result);
    }

    [Fact]
    public async Task GetShows_too_many_requests_returns_too_many_requests_response()
    {
        // Arrange
        using var handler = new MockHttpMessageHandler(HttpStatusCode.TooManyRequests);
        using var client = new HttpClient(handler) { BaseAddress = new Uri("https://localhost") };
        var sut = new TvMazeHttpService(client);

        // Act
        var result = await sut.GetShowsAsync(1, default);

        // Assert
        Assert.IsType<TooManyShowsRequestServiceResult>(result);
    }
    
    [Fact]
    public async Task GetShows_server_error_throws_exception()
    {
        // Arrange
        using var handler = new MockHttpMessageHandler(HttpStatusCode.InternalServerError);
        using var client = new HttpClient(handler) { BaseAddress = new Uri("https://localhost") };
        var sut = new TvMazeHttpService(client);

        // Act && Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => sut.GetShowsAsync(1, default));
    }    
    
    [Fact]
    public async Task GetCast_ok_returns_ok_response()
    {
        // Arrange
        using var handler = new MockHttpMessageHandler(HttpStatusCode.OK);
        using var client = new HttpClient(handler) { BaseAddress = new Uri("https://localhost") };
        var sut = new TvMazeHttpService(client);

        // Act
        var result = await sut.GetCastAsync(1, default);

        // Assert
        Assert.IsType<CastServiceResult>(result);
    }

    [Fact]
    public async Task GetCast_not_found_returns_not_found_response()
    {
        // Arrange
        using var handler = new MockHttpMessageHandler(HttpStatusCode.NotFound);
        using var client = new HttpClient(handler) { BaseAddress = new Uri("https://localhost") };
        var sut = new TvMazeHttpService(client);

        // Act
        var result = await sut.GetCastAsync(1, default);

        // Assert
        Assert.IsType<CastNotFoundServiceResult>(result);
    }

    [Fact]
    public async Task GetCast_too_many_requests_returns_too_many_requests_response()
    {
        // Arrange
        using var handler = new MockHttpMessageHandler(HttpStatusCode.TooManyRequests);
        using var client = new HttpClient(handler) { BaseAddress = new Uri("https://localhost") };
        var sut = new TvMazeHttpService(client);

        // Act
        var result = await sut.GetCastAsync(1, default);

        // Assert
        Assert.IsType<TooManyCastsRequestServiceResult>(result);
    }
    
    [Fact]
    public async Task GetCast_server_error_throws_exception()
    {
        // Arrange
        using var handler = new MockHttpMessageHandler(HttpStatusCode.InternalServerError);
        using var client = new HttpClient(handler) { BaseAddress = new Uri("https://localhost") };
        var sut = new TvMazeHttpService(client);

        // Act && Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => sut.GetCastAsync(1, default));
    }

    private class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _result;

        public MockHttpMessageHandler(HttpStatusCode result)
        {
            _result = result;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_result == HttpStatusCode.OK)
            {
                return Task.FromResult(new HttpResponseMessage(_result)
                {
                    Content = new StringContent("[]")
                });
            }

            return Task.FromResult(new HttpResponseMessage(_result));
        }
    }
}