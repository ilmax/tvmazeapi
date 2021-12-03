namespace TvMaze.Domain;

public class Show
{
    // Used by EF Core to deserialize the entity
    private Show() { }

    public Show(long id, string name, int page)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
        if (string.IsNullOrEmpty(name)) throw new ArgumentException("Value cannot be null or empty.", nameof(name));
        if (page <= 0) throw new ArgumentOutOfRangeException(nameof(page));
        Id = id;
        Name = name;
        Page = page;
    }
    
    public long Id { get; private set; }
    public string Name { get; private set; }
    public int Page { get; private set; }
    public bool HasNoCastInTheApi { get; private set; }
    public IEnumerable<Actor> Cast { get; private set; }

    public void SetCast(IEnumerable<Actor> cast)
    {
        if (cast.Any())
        {
            Cast = cast.ToList();
        }
        else
        {
            HasNoCastInTheApi = true;
        }
    }
}