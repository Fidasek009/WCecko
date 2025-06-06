namespace WCecko.Model.Rating;

using WCecko.Model.User;


public class RatingService(DatabaseService db, UserService userService)
{
    private readonly RatingDatabaseService _ratingDatabaseService = new(db.GetConnection());
    private readonly UserService _userService = userService;


    public async Task<Rating?> CreateRatingAsync(int placeId, int stars, string comment)
    {
        User? user = _userService.CurrentUser;
        if (user is null)
            return null;

        if (!user.HasPermission(UserPermission.CreateRatings))
            return null;

        return await _ratingDatabaseService.CreateRatingAsync(placeId, user.Username, stars, comment);
    }

    public async Task<Rating?> GetRatingAsync(int id)
    {
        Rating? rating = await _ratingDatabaseService.GetRatingAsync(id);
        if (rating is null)
            return null;

        rating.ModifyPermission = CheckModifyPermissions(rating.CreatedBy);
        return rating;
    }

    private bool CheckModifyPermissions(string creator)
    {
        User? user = _userService.CurrentUser;
        if (user is null)
            return false;

        if (user.HasPermission(UserPermission.ModifyAllRatings))
            return true;

        if (user.Username == creator && user.HasPermission(UserPermission.ModifyOwnRatings))
            return true;

        return false;
    }

    public async Task<bool> DeleteRatingAsync(Rating rating)
    {
        if (!CheckModifyPermissions(rating.CreatedBy))
            return false;

        return await _ratingDatabaseService.DeleteRatingAsync(rating.Id);
    }

    public async Task<bool> UpdateRatingAsync(Rating rating)
    {
        if (!CheckModifyPermissions(rating.CreatedBy))
            return false;

        return await _ratingDatabaseService.UpdateRatingAsync(rating);
    }

    public async Task<IReadOnlyList<Rating>> GetPlaceRatingsAsync(int placeId)
    {
        IReadOnlyList<Rating> ratings = await _ratingDatabaseService.GetPlaceRatingsAsync(placeId);

        // inject modify permissions
        foreach (Rating rating in ratings)
            rating.ModifyPermission = CheckModifyPermissions(rating.CreatedBy);

        return ratings;
    }
}
