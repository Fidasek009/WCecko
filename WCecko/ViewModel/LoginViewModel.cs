using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Runtime.Versioning;

namespace WCecko.ViewModel;

public partial class LoginViewModel : ObservableObject
{
    private const string USERNAME = "admin";
    private const string PASSWORD = "admin";

    [ObservableProperty]
    public partial string Username { get; set; } = "";

    [ObservableProperty]
    public partial string Password { get; set; } = "";

    [RelayCommand]
    async Task Login()
    {
        if (Username == USERNAME && Password == PASSWORD)
        {
            await Shell.Current.GoToAsync($"//{nameof(MainPage)}",
                new Dictionary<string, object>
                {
                    { "Username", Username }
                }
            );
        }
        else
        {
            await Shell.Current.DisplayAlert("Error", "Invalid username or password", "OK");
        }
    }

    [RelayCommand]
    async Task Register()
    {
        await Shell.Current.GoToAsync(nameof(RegisterPage));
    }
}
