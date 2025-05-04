using SQLite;

namespace WCecko.Model.Rating;

public class Rating
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed, NotNull]
    public int PlaceId { get; set; }

    [Indexed, NotNull]
    public string CreatedBy { get; set; } = "";

    [NotNull]
    public int Stars { get; set; }

    [NotNull]
    public string Comment { get; set; } = "";

    [Ignore]
    public bool ModifyPermission { get; set; } = true;  // TODO: set to false later
}
