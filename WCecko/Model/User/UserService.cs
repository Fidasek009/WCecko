namespace WCecko.Model.User;

public class UserService(UserDatabaseService userDatabaseService)
{
    private readonly UserDatabaseService _userDatabaseService = userDatabaseService;
    public event EventHandler<User?> UserChanged = delegate { };

    private User? _currentUser;
    public User? CurrentUser
    {
        get => _currentUser;
        private set
        {
            _currentUser = value;
            UserChanged?.Invoke(this, _currentUser);
        }
    }

    public async Task<bool> RegisterUserAsync(string username, string password)
    {
        if (await _userDatabaseService.RegisterUserAsync(username, password))
            return await AuthenticateUserAsync(username, password);
        return false;
    }

    public async Task<bool> AuthenticateUserAsync(string username, string password)
    {
        var result = await _userDatabaseService.AuthenticateUserAsync(username, password);
        if (result)
        {
            CurrentUser = new User
            {
                Username = username,
                PasswordHash = password,
                Role = UserRole.User
            };
        }
        return result;
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
