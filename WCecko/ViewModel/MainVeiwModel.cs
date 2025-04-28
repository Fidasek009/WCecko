using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace WCecko.ViewModel
{
    [QueryProperty("Username", "Username")]
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        string _username = "";

        [RelayCommand]
        async Task Logout()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
