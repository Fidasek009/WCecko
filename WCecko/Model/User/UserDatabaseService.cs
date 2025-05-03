using SQLite;
using BCryptHelper = BCrypt.Net.BCrypt;

namespace WCecko.Model.User;

public class UserDatabaseService(SQLiteAsyncConnection db)
{
    private readonly SQLiteAsyncConnection _db = db;

    public async Task<bool> RegisterUserAsync(string username, string password)
    {
        var existingUser = await _db.Table<User>()
            .Where(u => u.Username == username)
            .FirstOrDefaultAsync();

        if (existingUser != null)
            return false;

        var passwordHash = BCryptHelper.HashPassword(password);
        var newUser = new User
        {
            Username = username,
            PasswordHash = passwordHash,
            Role = UserRole.User
        };

        await _db.InsertAsync(newUser);
        return true;
    }

    public async Task<bool> AuthenticateUserAsync(string username, string password)
    {
        var user = await _db.Table<User>()
            .Where(u => u.Username == username)
            .FirstOrDefaultAsync();
        
        if (user == null)
            return false;
        
        return BCryptHelper.Verify(password, user.PasswordHash);
    }
}
