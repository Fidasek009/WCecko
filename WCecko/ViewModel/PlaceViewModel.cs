using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WCecko.ViewModel;

public partial class PlaceViewModel : ObservableObject
{
    private readonly IPopupService _popupService;

    public PlaceViewModel(IPopupService popupService)
    {
        _popupService = popupService;
    }

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
        var result = await _popupService.ShowPopupAsync<CreatePlaceViewModel>();
        if (result is not CreatePlaceViewModel resultViewModel)
            return;

        Name = resultViewModel.PlaceName;
        Description = resultViewModel.PlaceDescription;
        PlaceImage = resultViewModel.PlaceImage;
    }

    [RelayCommand]
    async Task DeletePlace()
    {
        // TODO
    }
}
