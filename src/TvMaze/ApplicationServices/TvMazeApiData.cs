using System.Text.Json.Serialization;

namespace TvMaze.ApplicationServices;

// [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
// [JsonSerializable(typeof(Show))]
// [JsonSerializable(typeof(CastEntry))]
// public partial class ShowContext : JsonSerializerContext
// {
// }

public record CastEntry
{
    public Person Person { get; set; }
    public Character Character { get; set; }
}

public record Character
{
    public long? Id { get; set; }
    public Uri Url { get; set; }
    public string Name { get; set; }
}

public record Person
{
    public long Id { get; set; }
    public Uri Url { get; set; }
    public string Name { get; set; }
    public Country Country { get; set; }
    public DateTimeOffset? Birthday { get; set; }
    public DateTimeOffset? Deathday { get; set; }
}

public record Show
{
    public long Id { get; set; }
    public Uri Url { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Language { get; set; }
    public string[] Genres { get; set; }
    public string Status { get; set; }
    public long? Runtime { get; set; }
    public long? AverageRuntime { get; set; }
    public DateTimeOffset? Premiered { get; set; }
    public DateTimeOffset? Ended { get; set; }
    public object OfficialSite { get; set; }
    public Schedule Schedule { get; set; }
    public Rating Rating { get; set; }
    public long? Weight { get; set; }
    public Image Image { get; set; }
    public string Summary { get; set; }
    public long? Updated { get; set; }
}

public record Image
{
    public Uri Medium { get; set; }
    public Uri Original { get; set; }
}

public record Network
{
    public long Id { get; set; }
    public string Name { get; set; }
    public Country Country { get; set; }
}

public record Country
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string Timezone { get; set; }
}

public record Rating
{
    public double? Average { get; set; }
}

public record Schedule
{
    public string Time { get; set; }
    public string[] Days { get; set; }
}