namespace WCecko.Model.User;

using SQLite;
using BCryptHelper = BCrypt.Net.BCrypt;


public class UserDatabaseService(SQLiteAsyncConnection db)
{
    private readonly SQLiteAsyncConnection _db = db;

    public async Task<User?> RegisterUserAsync(string username, string password)
    {
        User existingUser = await _db.Table<User>()
            .Where(u => u.Username == username)
            .FirstOrDefaultAsync();

        if (existingUser is not null)
            return null;

        User newUser = new()
        {
            Username = username,
            PasswordHash = BCryptHelper.HashPassword(password),
            Role = UserRole.User
        };

        await _db.InsertAsync(newUser);
        return newUser;
    }

    public async Task<User?> AuthenticateUserAsync(string username, string password)
    {
        User user = await _db.Table<User>()
            .Where(u => u.Username == username)
            .FirstOrDefaultAsync();

        if (user is null)
            return null;

        if (!BCryptHelper.Verify(password, user.PasswordHash))
            return null;

        return user;
    }
}
