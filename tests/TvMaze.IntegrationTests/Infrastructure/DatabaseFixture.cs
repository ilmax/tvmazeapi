using System;
using System.Linq;
using System.Threading.Tasks;
using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Configurations.Databases;
using Microsoft.EntityFrameworkCore;
using TvMaze.Data;
using TvMaze.Domain;
using Xunit;

namespace TvMaze.IntegrationTests.Infrastructure;

public class DatabaseFixture : IAsyncLifetime
{
    private readonly SqlServerTestcontainer _container;

    public DatabaseFixture()
    {
        var builder = new TestcontainersBuilder<SqlServerTestcontainer>()
            .WithName("sql-server-db")
            .WithDatabase(new MsSqlTestcontainerConfiguration("mcr.microsoft.com/mssql/server:2019-latest")
            {
                Password = "Guess_me",
                Port = 1535
            });

        _container = builder.Build();
    }

    public string ContainerConnectionString => _container.ConnectionString;

    public TvMazeDbContext CreateContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<TvMazeDbContext>()
            .UseSqlServer(_container.ConnectionString);
        return new TvMazeDbContext(optionsBuilder.Options);
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        var optionsBuilder = new DbContextOptionsBuilder<TvMazeDbContext>()
            .UseSqlServer(_container.ConnectionString);

        await using var context = new TvMazeDbContext(optionsBuilder.Options);

        await context.Database.EnsureCreatedAsync();
        await SeedInitialDataAsync(context);
    }

    private async Task SeedInitialDataAsync(TvMazeDbContext context)
    {
        var cast = new[]
        {
            new Actor(1, "Actor1", new DateTime(1970, 1, 1)),
            new Actor(2, "Actor2", new DateTime(1950, 1, 1)),
            new Actor(3, "Actor3", new DateTime(1980, 1, 1)),
            new Actor(4, "Actor4", new DateTime(1990, 1, 1)),
            new Actor(5, "Actor5", null),
        };
        var show1 = new Show(1, "Show 1", 1);
        show1.SetCast(cast.Take(2));

        var show2 = new Show(2, "Show 2", 1);
        show2.SetCast(cast.Skip(2).Take(2));

        var show3 = new Show(3, "Show 3", 1);
        show3.SetCast(cast.Skip(1));

        var show4 = new Show(4, "Show 4", 1);
        
        var show5 = new Show(5, "Show5", 1);
        show5.SetCast(Enumerable.Empty<Actor>());
        
        var show6 = new Show(6, "Show 6", 1);

        var run1 = new JobRunMetadata(DateTimeOffset.UtcNow.AddDays(-1), RunStatus.RunSuccessful, 1);
        var run2 = new JobRunMetadata(DateTimeOffset.UtcNow, RunStatus.RateLimitOnCasts, 1);

        context.AddRange(show1, show2, show3, show4, show5, show6, run1, run2);

        await context.SaveChangesAsync();
    }

    public async Task DisposeAsync() => await _container.StopAsync();
}