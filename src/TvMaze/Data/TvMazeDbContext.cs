using Microsoft.EntityFrameworkCore;
using TvMaze.Domain;

namespace TvMaze.Data;

public class TvMazeDbContext : DbContext
{
    public TvMazeDbContext(DbContextOptions options)
        : base(options)
    { }

    public DbSet<Show> Shows => Set<Show>();
    public DbSet<JobRunMetadata> JobRunMetadata => Set<JobRunMetadata>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Show>().Navigation(s => s.Cast).AutoInclude();
        modelBuilder.Entity<Show>().Property(x => x.Id).ValueGeneratedNever();
        modelBuilder.Entity<Show>().HasMany(x => x.Cast).WithMany("Shows");
        modelBuilder.Entity<Actor>().Property(x => x.Id).ValueGeneratedNever();
        modelBuilder.Entity<Actor>().HasIndex(x => x.DateOfBirth);
        modelBuilder.Entity<Actor>().ToTable("Actors");
        modelBuilder.Entity<JobRunMetadata>().HasIndex(x => new { x.RunAtUtc });
        modelBuilder.Entity<JobRunMetadata>().HasKey("Id");
    }
}