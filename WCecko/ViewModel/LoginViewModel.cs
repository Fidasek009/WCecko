using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WCecko.ViewModel
{
    public partial class LoginViewModel : ObservableObject
    {
        private const string USERNAME = "admin";
        private const string PASSWORD = "admin";

        [ObservableProperty]
        string _username = "";

        [ObservableProperty]
        string _password = "";

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
    }
}
