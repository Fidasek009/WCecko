using Mapsui;
using SQLite;

namespace WCecko.Model.Map;

public class MapDatabaseService(SQLiteAsyncConnection db)
{
    private readonly SQLiteAsyncConnection _db = db;

    public async Task<Place?> CreatePlaceAsync(MPoint mPoint, string username, string title, string description, ImageSource? image)
    {
        var newPlace = new Place
        {
            Location = mPoint,
            Title = title,
            Description = description,
            ImagePath = image != null ? await ImageUtils.SaveImageAsync(image, $"{Guid.NewGuid()}.jpg") : null,
            CreatedBy = username,
        };

        try
        {
            await _db.InsertAsync(newPlace);
            return newPlace;
        }
        catch (SQLiteException ex)
        {
            // TODO: log error
            Console.WriteLine($"Error inserting map point: {ex.Message}");
            return null;
        }
    }

    public async Task<Place?> GetPlaceAsync(int id)
    {
        return await _db.Table<Place>().FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IReadOnlyList<Place>> GetAllPlacesAsync()
    {
        var places = await _db.Table<Place>().ToListAsync();
        return places.AsReadOnly();
    }
}
