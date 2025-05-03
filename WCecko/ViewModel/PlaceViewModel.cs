using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using WCecko.Model.Map;
using WCecko.Model.User;

namespace WCecko.ViewModel;

[QueryProperty("PointId", "PointId")]
public partial class PlaceViewModel(IPopupService popupService, MapService mapService, UserService userService) : ObservableObject
{
    private readonly IPopupService _popupService = popupService;
    private readonly MapService _mapService = mapService;
    private readonly UserService _userService = userService;


    [ObservableProperty]
    public partial int PointId { get; set; }

    [ObservableProperty]
    public partial bool ModifyPermission { get; set; } = false;

    [ObservableProperty]
    public partial string Name { get; set; } = "";

    [ObservableProperty]
    public partial string Description { get; set; } = "";

    [ObservableProperty]
    public partial ImageSource? PlaceImage { get; set; }


    async partial void OnPointIdChanged(int value)
    {
        var point = await _mapService.GetMapPointAsync(value);
        if (point == null)
            return;

        Name = point.Title;
        Description = point.Description;
        PlaceImage = ImageSource.FromFile(point.ImagePath);
        ModifyPermission = CheckModifyPermission(point.CreatedBy);
    }


    private bool CheckModifyPermission(string pointCreator)
    {
        var user = _userService.CurrentUser;
        if (user == null)
            return false;

        if (user.HasPermission(UserPermission.ModifyAllPoints))
            return true;

        if (user.Username == pointCreator && user.HasPermission(UserPermission.ModifyOwnPoints))
            return true;

        return false;
    }


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
