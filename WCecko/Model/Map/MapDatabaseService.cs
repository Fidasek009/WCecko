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

    public async Task<bool> DeletePlaceAsync(int id)
    {
        var place = await GetPlaceAsync(id);
        if (place == null)
            return false;

        if (place.ImagePath != null)
            File.Delete(place.ImagePath);

        return await _db.DeleteAsync<Place>(id) > 0;
    }

    public async Task<bool> UpdatePlaceAsync(Place place)
    {
        var existingPlace = await GetPlaceAsync(place.Id);
        if (existingPlace == null)
            return false;

        if (existingPlace.ImagePath != null && place.ImagePath != existingPlace.ImagePath)
            File.Delete(existingPlace.ImagePath);

        var newPlace = new Place
        {
            Id = place.Id,
            Location = place.Location,
            Title = place.Title,
            Description = place.Description,
            ImagePath = place.ImagePath != null ? await ImageUtils.SaveImageAsync(place.ImagePath, $"{Guid.NewGuid()}.jpg") : null,
            CreatedBy = existingPlace.CreatedBy,
        };

        return await _db.UpdateAsync(place) > 0;
    }

    public async Task<IReadOnlyList<Place>> GetAllPlacesAsync()
    {
        var places = await _db.Table<Place>().ToListAsync();
        return places.AsReadOnly();
    }
}
