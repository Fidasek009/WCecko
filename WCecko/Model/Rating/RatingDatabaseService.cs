

using SQLite;

namespace WCecko.Model.Rating;

public class RatingDatabaseService(SQLiteAsyncConnection db)
{
    private readonly SQLiteAsyncConnection _db = db;

    public async Task<Rating?> CreateRatingAsync(int placeId, string username, int stars, string comment)
    {
        var newRating = new Rating
        {
            PlaceId = placeId,
            Stars = stars,
            Comment = comment,
            CreatedBy = username,
        };

        try
        {
            await _db.InsertAsync(newRating);
            return newRating;
        }
        catch (SQLiteException ex)
        {
            // TODO: log error
            Console.WriteLine($"Error inserting rating: {ex.Message}");
            return null;
        }
    }

    public async Task<Rating?> GetRatingAsync(int id)
    {
        return await _db.Table<Rating>().FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> DeleteRatingAsync(int id)
    {
        return await _db.DeleteAsync<Rating>(id) > 0;
    }

    public async Task<IReadOnlyList<Rating>> GetPlaceRatingsAsync(int placeId)
    {
        var ratings = await _db.Table<Rating>().Where(x => x.PlaceId == placeId).ToListAsync();
        return ratings.AsReadOnly();
    }
}
