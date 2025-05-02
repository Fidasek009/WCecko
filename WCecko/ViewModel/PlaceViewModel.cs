using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WCecko.ViewModel;

public partial class PlaceViewModel : ObservableObject
{
    [ObservableProperty]
    public partial bool ModifyPermission { get; set; } = true; // TODO: set to false later
    
    [ObservableProperty]
    public partial string Name { get; set; } = "";

    [ObservableProperty]
    public partial string Description { get; set; } = "";

    [ObservableProperty]
    public partial ImageSource? PlaceImage { get; set; }


    [RelayCommand]
    async Task EditPlace()
    {
        // TODO
    }

    [RelayCommand]
    async Task DeletePlace()
    {
        // TODO
    }
}
