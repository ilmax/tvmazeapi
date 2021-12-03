using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TvMaze.Data;
using TvMaze.IntegrationTests.Infrastructure;
using TvMaze.Models;
using Xunit;

namespace TvMaze.IntegrationTests.Web;

[Collection("db")]
public class ApiTests : BaseWebApplicationTest
{
    public ApiTests(DatabaseFixture fixture) :
        base(services =>
        {
            services.RemoveAll<TvMazeDbContext>();
            services.RemoveAll<DbContextOptions<TvMazeDbContext>>();
            services.AddDbContext<TvMazeDbContext>(opt => opt.UseSqlServer(fixture.ContainerConnectionString));
        })
    {
    }

    [Fact]
    public async Task GetShows_returns_a_paginated_result()
    {
        // Act
        var result = await Client.GetFromJsonAsync<PageResult<ShowModel>>("shows?pageSize=2");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(6, result.TotalCount);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal(1, result.CurrentPage);
    }

    [Fact]
    public async Task GetShows_can_be_paginated()
    {
        // Act
        var result = await Client.GetFromJsonAsync<PageResult<ShowModel>>("shows?page=2&pageSize=2");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(6, result.TotalCount);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal(2, result.CurrentPage);
    }

    [Fact]
    public async Task GetShows_page_out_of_range_returns_404()
    {
        // Act
        var result = await Client.GetAsync("shows?page=22&pageSize=2");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task GetShows_wrong_page_parameter_returns_400()
    {
        // Act
        var result = await Client.GetAsync("shows?page=0&pageSize=2");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task GetShows_wrong_pageSize_parameter_returns_400()
    {
        // Act
        var result = await Client.GetAsync("shows?page=1&pageSize=0");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }
}