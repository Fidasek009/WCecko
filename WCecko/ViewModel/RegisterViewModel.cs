using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WCecko.ViewModel
{
    public partial class RegisterViewModel : ObservableObject
    {
        [ObservableProperty]
        string _username = "";

        [ObservableProperty]
        string _password = "";

        [ObservableProperty]
        string _confirmPassword = "";

        [RelayCommand]
        async Task Register()
        {
            await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        }
    }
}
