namespace WCecko.Model.User;

/// <summary>
/// Enum representing user permissions.
/// </summary>
/// <remarks>
/// View permissions are not included because every user can view everything. (at the moment)
/// </remarks>
public enum UserPermission
{
    CreatePlaces,
    ModifyOwnPlaces,
    ModifyAllPlaces,
    CreateRatings,
    ModifyOwnRatings,
    ModifyAllRatings,
}
