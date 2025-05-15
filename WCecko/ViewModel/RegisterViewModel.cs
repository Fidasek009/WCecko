using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using WCecko.Model.User;

namespace WCecko.ViewModel;

public partial class RegisterViewModel(UserService userService) : ObservableObject
{
    private readonly UserService _userService = userService;


    [ObservableProperty]
    public partial string Username { get; set; } = "";

    [ObservableProperty]
    public partial string Password { get; set; } = "";

    [ObservableProperty]
    public partial string ConfirmPassword { get; set; } = "";


    [RelayCommand]
    async Task Register()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.DisplayAlert("Error", "Username and password cannot be empty", "OK");
            return;
        }

        if (Password != ConfirmPassword)
        {
            await Shell.Current.DisplayAlert("Error", "Passwords do not match", "OK");
            return;
        }

        var result = await _userService.RegisterUserAsync(Username, Password);

        if (!result)
        {
            await Shell.Current.DisplayAlert("Error", "Registration failed. Username may already be taken.", "OK");
            return;
        }

        await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
    }
}
