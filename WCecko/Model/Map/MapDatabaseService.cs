using Mapsui;
using SQLite;

namespace WCecko.Model.Map;

public class MapDatabaseService(SQLiteAsyncConnection db)
{
    private readonly SQLiteAsyncConnection _db = db;

    public async Task<int?> CreateMapPointAsync(MPoint mPoint, string username, string title, string description, ImageSource? image)
    {
        var newPoint = new MapPoint
        {
            Location = mPoint,
            Title = title,
            Description = description,
            ImagePath = image != null ? await ImageUtils.SaveImageAsync(image, $"{Guid.NewGuid()}.jpg") : null,
            CreatedBy = username,
        };

        try
        {
            await _db.InsertAsync(newPoint);
            return newPoint.Id;
        }
        catch (SQLiteException ex)
        {
            // TODO: log error
            Console.WriteLine($"Error inserting map point: {ex.Message}");
            return null;
        }
    }

    public async Task<MapPoint?> GetMapPointAsync(int id)
    {
        return await _db.Table<MapPoint>().FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IReadOnlyList<MapPoint>> GetAllMapPointsAsync()
    {
        var mapPoints = await _db.Table<MapPoint>().ToListAsync();
        return mapPoints.AsReadOnly();
    }
}
