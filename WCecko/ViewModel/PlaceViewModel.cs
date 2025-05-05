using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using WCecko.Model.Map;
using WCecko.Model.Rating;
using WCecko.Model.User;
using WCecko.View;

namespace WCecko.ViewModel;

[QueryProperty("PlaceId", "PlaceId")]
public partial class PlaceViewModel(IPopupService popupService, MapService mapService, UserService userService, RatingService ratingService) : ObservableObject
{
    private readonly IPopupService _popupService = popupService;
    private readonly MapService _mapService = mapService;
    private readonly UserService _userService = userService;


    public RatingsViewModel RatingsViewModel { get; } = new RatingsViewModel(popupService, ratingService);

    [ObservableProperty]
    public partial int PlaceId { get; set; }

    [ObservableProperty]
    public partial bool ModifyPermission { get; set; } = false;

    [ObservableProperty]
    public partial string Name { get; set; } = "";

    [ObservableProperty]
    public partial string Description { get; set; } = "";

    [ObservableProperty]
    public partial ImageSource? PlaceImage { get; set; }


    async partial void OnPlaceIdChanged(int value)
    {
        var place = await _mapService.GetPlaceAsync(value);
        if (place == null)
            return;

        Name = place.Title;
        Description = place.Description;
        PlaceImage = ImageSource.FromFile(place.ImagePath);
        ModifyPermission = CheckModifyPermission(place.CreatedBy);
        this.RatingsViewModel.PlaceId = value;
    }


    private bool CheckModifyPermission(string pointCreator)
    {
        var user = _userService.CurrentUser;
        if (user == null)
            return false;

        if (user.HasPermission(UserPermission.ModifyAllPlaces))
            return true;

        if (user.Username == pointCreator && user.HasPermission(UserPermission.ModifyOwnPlaces))
            return true;

        return false;
    }


    [RelayCommand]
    async Task EditPlace()
    {
        var result = await _popupService.ShowPopupAsync<CreatePlaceViewModel>(
            onPresenting: vm =>
            {
                vm.PlaceName = Name;
                vm.PlaceDescription = Description;
                vm.PlaceImage = PlaceImage;
            });

        // var result = await _popupService.ShowPopupAsync<CreatePlaceViewModel>();
        if (result is not CreatePlaceViewModel resultViewModel)
            return;

        var editResult = await _mapService.UpdatePlaceAsync(
            PlaceId,
            resultViewModel.PlaceName,
            resultViewModel.PlaceDescription,
            resultViewModel.PlaceImage);

        if (!editResult)
        {
            await Shell.Current.DisplayAlert("Error", "Failed to edit place.", "OK");
            return;
        }

        Name = resultViewModel.PlaceName;
        Description = resultViewModel.PlaceDescription;
        PlaceImage = resultViewModel.PlaceImage;
    }

    [RelayCommand]
    async Task DeletePlace()
    {
        var confirm = await Shell.Current.DisplayAlert("Delete Place", "Are you sure you want to delete this place?", "Yes", "No");
        if (!confirm)
            return;

        var result = await _mapService.DeletePlaceAsync(PlaceId);
        if (!result)
        {
            await Shell.Current.DisplayAlert("Error", "Failed to delete place.", "OK");
            return;
        }

        await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
    }
}
