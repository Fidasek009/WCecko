namespace WCecko.Model.Map;

using Mapsui;
using SQLite;


public class MapDatabaseService(SQLiteAsyncConnection db)
{
    private readonly SQLiteAsyncConnection _db = db;

    public async Task<Place?> CreatePlaceAsync(MPoint mPoint, string username, string title, string description, ImageSource? image)
    {
        Place newPlace = new()
        {
            Location = mPoint,
            Title = title,
            Description = description,
            ImagePath = image is not null ? await ImageUtils.SaveImageAsync(image, $"{Guid.NewGuid()}.jpg") : null,
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
        Place? place = await GetPlaceAsync(id);
        if (place is null)
            return false;

        if (place.ImagePath is not null)
            File.Delete(place.ImagePath);

        return await _db.DeleteAsync<Place>(id) > 0;
    }

    public async Task<bool> UpdatePlaceAsync(Place place)
    {
        Place? existingPlace = await GetPlaceAsync(place.Id);
        if (existingPlace is null)
            return false;

        if (existingPlace.ImagePath is not null && place.ImagePath != existingPlace.ImagePath)
            File.Delete(existingPlace.ImagePath);

        Place newPlace = new()
        {
            Id = place.Id,
            Location = place.Location,
            Title = place.Title,
            Description = place.Description,
            ImagePath = place.ImagePath is not null ? await ImageUtils.SaveImageAsync(place.ImagePath, $"{Guid.NewGuid()}.jpg") : null,
            CreatedBy = existingPlace.CreatedBy,
        };

        return await _db.UpdateAsync(place) > 0;
    }

    public async Task<IReadOnlyList<Place>> GetAllPlacesAsync()
    {
        List<Place> places = await _db.Table<Place>().ToListAsync();
        return places.AsReadOnly();
    }
}
