using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WCecko.ViewModel;

public partial class RegisterViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string Username { get; set; } = "";
    
    [ObservableProperty]
    public partial string Password { get; set; } = "";
    
    [ObservableProperty]
    public partial string ConfirmPassword { get; set; } = "";
    
    [RelayCommand]
    async Task Register()
    {
        await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
    }
}
