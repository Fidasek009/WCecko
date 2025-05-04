using SQLite;

namespace WCecko.Model.Rating;

public class Rating
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed, NotNull]
    public int PlacetId { get; set; }

    [Indexed, NotNull]
    public string Username { get; set; } = "";

    [NotNull]
    public string Comment { get; set; } = "";

    [NotNull]
    public int Stars { get; set; }

    [Ignore]
    public bool ModifyPermission { get; set; } = true;  // TODO: set to false later
}
