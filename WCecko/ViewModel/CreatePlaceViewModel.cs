
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mapsui;

namespace WCecko.ViewModel;

public partial class CreatePlaceViewModel : ObservableObject
{
    readonly IPopupService popupService;

    public CreatePlaceViewModel(IPopupService popupService)
    {
        this.popupService = popupService;
    }

    [ObservableProperty]
    public partial string PlaceName { get; set; } = "";

    [ObservableProperty]
    public partial string PlaceDescription { get; set; } = "";

    //[ObservableProperty]
    //public partial string PlaceImage { get; set; } = "";

    [RelayCommand]
    async Task Cancel()
    {
        await popupService.ClosePopupAsync(null);
    }

    [RelayCommand]
    async Task Save()
    {
        await popupService.ClosePopupAsync(this);
    }
}
