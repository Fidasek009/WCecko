namespace WCecko.Model.User;

public class UserService(DatabaseService db)
{
    private readonly UserDatabaseService _userDatabaseService = new(db.GetConnection());
    public event EventHandler<User?> UserChanged = delegate { };

    private User? _currentUser;
    public User? CurrentUser
    {
        get => _currentUser;
        private set
        {
            _currentUser = value;
            UserChanged.Invoke(this, _currentUser);
        }
    }

    public async Task<bool> RegisterUserAsync(string username, string password)
    {
        User? user = await _userDatabaseService.RegisterUserAsync(username, password);
        CurrentUser = user;
        return user != null;
    }

    public async Task<bool> AuthenticateUserAsync(string username, string password)
    {
        User? user = await _userDatabaseService.AuthenticateUserAsync(username, password);
        CurrentUser = user;
        return user != null;
    }

    public void Logout()
    {
        CurrentUser = null;
    }

    public bool HasPermission(UserPermission permission)
    {
        return CurrentUser?.HasPermission(permission) ?? false;
    }
}
