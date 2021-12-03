using System.Text.Json.Serialization;

namespace TvMaze.Models;

public record ActorModel
{
    public long Id { get; set; }
    public string Name { get; set; }
    public DateTimeOffset? DateOfBirth { get; set; }
}