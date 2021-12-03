using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TvMaze.ApplicationServices;
using TvMaze.Domain;
using Xunit;
using Show = TvMaze.ApplicationServices.Show;

namespace TvMaze.Tests;

public class ScraperServiceTests
{
    [Fact]
    public void Ctor_checks_for_null_tvMazeApiService()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ScraperService(null, Mock.Of<IShowManager>(), new NullLogger<ScraperService>()));
    }
    
    [Fact]
    public void Ctor_checks_for_null_showManager()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ScraperService(Mock.Of<ITvMazeHttpService>(), null, new NullLogger<ScraperService>()));
    }
    
    [Fact]
    public async Task RunAsync_can_start_from_missing_run_metadata()
    {
        // Arrange
        var apiMock = new Mock<ITvMazeHttpService>();
        apiMock.Setup(x => x.GetShowsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ShowsServiceResult{  Shows = Enumerable.Empty<Show>()});
        var managerMock = new Mock<IShowManager>();
        var sut = new ScraperService(apiMock.Object, managerMock.Object, new NullLogger<ScraperService>());
        
        // Act
        await sut.RunAsync(default);
        
        // Assert
        apiMock.Verify(x => x.GetShowsAsync(1, It.IsAny<CancellationToken>()));
    }    
    
    [Fact]
    public async Task RunAsync_will_fetch_next_page_after_a_successful_run()
    {
        // Arrange
        var apiMock = new Mock<ITvMazeHttpService>();
        apiMock.Setup(x => x.GetShowsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ShowsServiceResult{  Shows = Enumerable.Empty<Show>()});
        var managerMock = new Mock<IShowManager>();
        managerMock.Setup(x => x.GetLastRunAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new JobRunMetadata(DateTimeOffset.UtcNow, RunStatus.RunSuccessful, 1));
        var sut = new ScraperService(apiMock.Object, managerMock.Object, new NullLogger<ScraperService>());
        
        // Act
        await sut.RunAsync(default);
        
        // Assert
        apiMock.Verify(x => x.GetShowsAsync(2, It.IsAny<CancellationToken>()));
    }
    
    [Fact]
    public async Task RunAsync_will_fetch_cast_after_fetching_shows()
    {
        // Arrange
        var apiMock = new Mock<ITvMazeHttpService>();
        apiMock.Setup(x => x.GetShowsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ShowsServiceResult{  Shows = new[] { new Show{ Id = 123 }}});
        apiMock.Setup(x => x.GetCastAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CastServiceResult{  Cast = Enumerable.Empty<CastEntry>()});
        var managerMock = new Mock<IShowManager>();
        managerMock.Setup(x => x.GetLastRunAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new JobRunMetadata(DateTimeOffset.UtcNow, RunStatus.RunSuccessful, 1));
        var sut = new ScraperService(apiMock.Object, managerMock.Object, new NullLogger<ScraperService>());
        
        // Act
        await sut.RunAsync(default);
        
        // Assert
        apiMock.Verify(x => x.GetCastAsync(123, It.IsAny<CancellationToken>()));
    }
    
    [Fact]
    public async Task RunAsync_will_continue_fetching_cast_when_resuming_from_RateLimitOnCasts()
    {
        // Arrange
        var apiMock = new Mock<ITvMazeHttpService>();
        apiMock.Setup(x => x.GetCastAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CastServiceResult{  Cast = Enumerable.Empty<CastEntry>()});
        var managerMock = new Mock<IShowManager>();
        managerMock.Setup(x => x.GetShowIdsWithoutCastAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { 123L });
        managerMock.Setup(x => x.GetLastRunAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new JobRunMetadata(DateTimeOffset.UtcNow, RunStatus.RateLimitOnCasts, 1));
        var sut = new ScraperService(apiMock.Object, managerMock.Object, new NullLogger<ScraperService>());
        
        // Act
        await sut.RunAsync(default);
        
        // Assert
        apiMock.Verify(x => x.GetCastAsync(123, It.IsAny<CancellationToken>()));
    }
    
    [Fact]
    public async Task RunAsync_will_store_metadata_after_get_shows()
    {
        // Arrange
        var shows = new[] { new Show { Id = 123 } };
        var apiMock = new Mock<ITvMazeHttpService>();
        apiMock.Setup(x => x.GetShowsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ShowsServiceResult{  Shows = shows });
        apiMock.Setup(x => x.GetCastAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CastServiceResult{  Cast = Enumerable.Empty<CastEntry>()});
        var managerMock = new Mock<IShowManager>();
        var sut = new ScraperService(apiMock.Object, managerMock.Object, new NullLogger<ScraperService>());
        
        // Act
        await sut.RunAsync(default);
        
        // Assert
        managerMock.Verify(x => 
            x.StoreShowsPageAsync(
                It.Is<JobRunMetadata>(j => j.Status == RunStatus.RunSuccessful && j.LastSuccessfulShowPageFetched == 1), 
                shows, 
                1,
                It.IsAny<CancellationToken>()));
    }
    
    [Fact]
    public async Task RunAsync_will_pause_after_rate_limit_on_shows()
    {
        // Arrange
        var apiMock = new Mock<ITvMazeHttpService>();
        apiMock.Setup(x => x.GetShowsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TooManyShowsRequestServiceResult());
        var managerMock = new Mock<IShowManager>();
        var sut = new ScraperService(apiMock.Object, managerMock.Object, new NullLogger<ScraperService>());
        
        // Act
        await sut.RunAsync(default);
        
        // Assert
        managerMock.Verify(x => x.StoreLastRunMetadataAsync(0, RunStatus.RateLimitOnShows, It.IsAny<CancellationToken>()));
        managerMock.Verify(x => x.StoreShowsPageAsync(
            It.IsAny<JobRunMetadata>(), 
            It.IsAny<IEnumerable<Show>>(), 
            It.IsAny<int>(), 
            It.IsAny<CancellationToken>()), Times.Never);
        apiMock.Verify(x => x.GetCastAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task RunAsync_will_pause_after_rate_limit_on_casts()
    {
        // Arrange
        var shows = new[] { new Show { Id = 123 } };
        var apiMock = new Mock<ITvMazeHttpService>();
        apiMock.Setup(x => x.GetShowsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ShowsServiceResult{  Shows = shows });
        apiMock.Setup(x => x.GetCastAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TooManyCastsRequestServiceResult());
        var managerMock = new Mock<IShowManager>();
        var sut = new ScraperService(apiMock.Object, managerMock.Object, new NullLogger<ScraperService>());
        
        // Act
        await sut.RunAsync(default);
        
        // Assert
        managerMock.Verify(x => x.StoreLastRunMetadataAsync(1, RunStatus.RateLimitOnCasts, It.IsAny<CancellationToken>()));
        managerMock.Verify(x => x.StoreCastForShowAsync(
            It.IsAny<JobRunMetadata>(), 
            It.IsAny<long>(), 
            It.IsAny<IEnumerable<CastEntry>>(), 
            It.IsAny<CancellationToken>()), Times.Never);
    }
}