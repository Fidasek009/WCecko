namespace WCecko.ViewModel;

using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


[QueryProperty("Stars", "Stars")]
[QueryProperty("Comment", "Comment")]
public partial class AddRatingViewModel(IPopupService popupService) : ObservableObject
{
    private readonly IPopupService _popupService = popupService;

    [ObservableProperty]
    public partial string Title { get; set; } = "Add rating";

    [ObservableProperty]
    public partial int Stars { get; set; } = 3;

    [ObservableProperty]
    public partial string Comment { get; set; } = "";

    [RelayCommand]
    private async Task Cancel()
    {
        await _popupService.ClosePopupAsync(null);
    }

    [RelayCommand]
    private async Task Save()
    {
        await _popupService.ClosePopupAsync(this);
    }
}
