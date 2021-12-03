using System.Linq;
using System.Threading.Tasks;
using TvMaze.ApplicationServices;
using TvMaze.IntegrationTests.Infrastructure;
using Xunit;

namespace TvMaze.IntegrationTests.Data;

[Collection("db")]
public class ShowProviderTests
{
    private readonly DatabaseFixture _databaseFixture;

    public ShowProviderTests(DatabaseFixture databaseFixture)
    {
        _databaseFixture = databaseFixture;
    }

    [Fact]
    public async Task GetShowAsync_returns_a_paginated_result_ordered_by_show_id()
    {
        // Arrange
        await using var context = _databaseFixture.CreateContext();
        var sut = new ShowProvider(new PaginationConfig { MaximumPageSize = 5 }, context);

        // Act
        var result = await sut.GetShowAsync(1, 5, default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(6, result.TotalCount);
        Assert.Equal(1, result.CurrentPage);
        Assert.Equal(1, result.Items.First().Id);
        Assert.Equal(5, result.Items.Last().Id);
    }

    [Fact]
    public async Task GetShowAsync_returns_a_paginated_with_cast_ordered_by_date_of_birth()
    {
        // Arrange
        await using var context = _databaseFixture.CreateContext();
        var sut = new ShowProvider(new PaginationConfig { MaximumPageSize = 5 }, context);

        // Act
        var result = await sut.GetShowAsync(1, 5, default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(6, result.TotalCount);
        Assert.Equal(1, result.CurrentPage);
        var firstShow = result.Items.First();
        Assert.Equal(1950, firstShow.Cast.First().Birthday.GetValueOrDefault().Year);
        Assert.Equal(1970, firstShow.Cast.Last().Birthday.GetValueOrDefault().Year);
    }

    [Fact]
    public async Task GetShowAsync_returns_a_paginated_with_cast_ordered_by_date_of_birth_nulls_first()
    {
        // Arrange
        await using var context = _databaseFixture.CreateContext();
        var sut = new ShowProvider(new PaginationConfig { MaximumPageSize = 5 }, context);

        // Act
        var result = await sut.GetShowAsync(1, 5, default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(6, result.TotalCount);
        Assert.Equal(1, result.CurrentPage);
        var lastShow = result.Items.First(x => x.Cast.Any(a => a.Birthday is null));
        Assert.Null(lastShow.Cast.First().Birthday);
        Assert.Equal(1950, lastShow.Cast.Skip(1).First().Birthday.GetValueOrDefault().Year);
    }

    [Fact]
    public async Task GetShowAsync_page_size_is_protected_by_config()
    {
        // Arrange
        await using var context = _databaseFixture.CreateContext();
        var sut = new ShowProvider(new PaginationConfig { MaximumPageSize = 2 }, context);

        // Act
        var result = await sut.GetShowAsync(1, int.MaxValue, default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(6, result.TotalCount);
        Assert.Equal(1, result.CurrentPage);
        Assert.Equal(2, result.Items.Count());
    }
}