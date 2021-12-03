using System.Text.Json.Serialization;
using TvMaze.ApplicationServices;

namespace TvMaze.Models;

public record ShowModel
{
    public long Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<ActorModel> Cast { get; set; }
}

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(ShowModel))]
internal partial class ShowModelContext : JsonSerializerContext
{
}