namespace WCecko.ViewModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using WCecko.Model.User;


public partial class LoginViewModel(UserService userService) : ObservableObject
{
    private readonly UserService _userService = userService;


    [ObservableProperty]
    public partial string Username { get; set; } = "";

    [ObservableProperty]
    public partial string Password { get; set; } = "";


    [RelayCommand]
    async Task Login()
    {
        bool result = await _userService.AuthenticateUserAsync(Username, Password);

        if (!result)
        {
            await Shell.Current.DisplayAlert("Error", "Invalid username or password", "OK");
            return;
        }

        await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
    }

    [RelayCommand]
    async Task Register()
    {
        await Shell.Current.GoToAsync(nameof(RegisterPage));
    }
}
