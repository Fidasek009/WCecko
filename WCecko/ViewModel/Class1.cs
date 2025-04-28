using CommunityToolkit.Mvvm.ComponentModel;


namespace WCecko.ViewModel
{
    [QueryProperty("Username", "Username")]
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        string _username = "";

    }
}
