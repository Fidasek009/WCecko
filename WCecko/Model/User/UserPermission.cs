namespace WCecko.Model.User;

/// <summary>
/// Enum representing user permissions.
/// </summary>
/// <remarks>
/// View permissions are not included because every user can view everything. (at the moment)
/// </remarks>
public enum UserPermission
{
    Unknown = 0,
    CreatePlaces = 1,
    ModifyOwnPlaces = 2,
    ModifyAllPlaces = 3,
    CreateRatings = 4,
    ModifyOwnRatings = 5,
    ModifyAllRatings = 6,
}
