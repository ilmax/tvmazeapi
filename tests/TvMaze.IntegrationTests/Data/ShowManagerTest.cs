using System.Linq;
using System.Threading.Tasks;
using TvMaze.ApplicationServices;
using TvMaze.Domain;
using TvMaze.IntegrationTests.Infrastructure;
using Xunit;

namespace TvMaze.IntegrationTests.Data;

[Collection("db")]
public class ShowManagerTest
{
    private readonly DatabaseFixture _databaseFixture;

    public ShowManagerTest(DatabaseFixture databaseFixture)
    {
        _databaseFixture = databaseFixture;
    }

    [Fact]
    public async Task GetShowIdsWithoutCastAsync_returns_only_shows_without_cast()
    {
        // Arrange
        await using var context = _databaseFixture.CreateContext();
        var sut = new ShowManager(context);

        // Act
        var result = await sut.GetShowIdsWithoutCastAsync(default);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Equal(4L, result.First());
        Assert.Equal(6L, result.Last());
    }
    
    [Fact]
    public async Task GetLastRunAsync_returns_last_run_order_by_date()
    {
        // Arrange
        await using var context = _databaseFixture.CreateContext();
        var sut = new ShowManager(context);

        // Act
        var result = await sut.GetLastRunAsync(default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(RunStatus.RateLimitOnCasts, result.Status);
    }
}