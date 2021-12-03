namespace TvMaze.Domain;

public class Actor
{
    public Actor(long id, string name, DateTimeOffset? dateOfBirth)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
        if (string.IsNullOrEmpty(name)) throw new ArgumentException("Value cannot be null or empty.", nameof(name));
        Id = id;
        Name = name;
        DateOfBirth = dateOfBirth;
    }

    public long Id { get; private set; }
    public string Name { get; private set; }
    public DateTimeOffset? DateOfBirth { get; private set; }

    // Required by EF to setup a many to many relationship with Shows
    private ICollection<Show> Shows { get; set; }
}
