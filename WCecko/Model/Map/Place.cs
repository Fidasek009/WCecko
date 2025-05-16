namespace WCecko.Model.Map;

using Mapsui;
using SQLite;


public class Place
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed(Name = "LocationIndex", Unique = true), NotNull]
    public double X { get; set; }

    [Indexed(Name = "LocationIndex", Unique = true), NotNull]
    public double Y { get; set; }

    [Indexed, NotNull]
    public string CreatedBy { get; set; } = "";

    public string Title { get; set; } = "";

    public string Description { get; set; } = "";


    public const int IMAGE_MAX_HEIGHT = 400;
    public const int IMAGE_MAX_WIDTH = 800;

    public string? ImagePath { get; set; }

    [Ignore]
    public MPoint Location
    {
        get => new(X, Y);
        set
        {
            X = value.X;
            Y = value.Y;
        }
    }
}
