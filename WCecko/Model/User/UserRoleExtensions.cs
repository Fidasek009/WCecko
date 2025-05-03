
namespace WCecko.Model.User;

public static class UserRoleExtensions
{
    private static readonly Dictionary<UserRole, UserPermission[]> _permissions = new()
    {
        {
            UserRole.Admin, new[] {
                UserPermission.CreatePoints,
                UserPermission.ModifyAllPoints,
                UserPermission.AddRatings,
                UserPermission.ModifyAllRatings
            }
        },
        {
            UserRole.User, new[] {
                UserPermission.CreatePoints,
                UserPermission.ModifyOwnPoints,
                UserPermission.AddRatings,
                UserPermission.ModifyOwnRatings
            }
        }
    };

    /// <summary>
    /// Checks if the user role has the specified permission.
    /// </summary>
    /// <param name="role">role of the user</param>
    /// <param name="permission">permission to check for</param>
    /// <returns>true if user has permission, false otherwise</returns>
    public static bool HasPermission(this UserRole role, UserPermission permission)
    {
        return _permissions.ContainsKey(role) && _permissions[role].Contains(permission);
    }

    /// <summary>
    /// Gets all the permissions for a specific user role.
    /// </summary>
    /// <param name="role">role of the user</param>
    /// <returns>array of permissions the user has</returns>
    public static UserPermission[] GetPermissions(this UserRole role)
    {
        return _permissions.GetValueOrDefault(role, []);
    }
}
