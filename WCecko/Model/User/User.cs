using SQLite;

namespace WCecko.Model.User;

public class User
{
    [PrimaryKey, NotNull]
    public string Username { get; set; } = "";

    [NotNull]
    public string PasswordHash { get; set; } = "";

    [NotNull]
    public UserRole Role { get; set; } = UserRole.User;


    /// <summary>
    /// Checks if the user has the specified permission.
    /// </summary>
    /// <param name="permission">permission to check for</param>
    /// <returns>true if user has permission, false otherwise</returns>
    public bool HasPermission(UserPermission permission)
    {
        return Role.HasPermission(permission);
    }
}
