using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.UI.Maui;
using System.Diagnostics;
using WCecko.Model;
using WCecko.Model.User;
using WCecko.View;
using TappedEventArgs = Mapsui.UI.TappedEventArgs;

namespace WCecko.ViewModel;

public partial class MainViewModel(IPopupService popupService, UserService userService) : ObservableObject
{
    private readonly IPopupService _popupService = popupService;
    private readonly UserService _userService = userService;


    [ObservableProperty]
    public partial string Username { get; set; } = userService.CurrentUser?.Username ?? "Guest";


    [RelayCommand]
    async Task Logout()
    {
        _userService.Logout();
        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }

    public async void OnMapLongTapped(object? sender, TappedEventArgs e)
    {
        try
        {
            var mapControl = sender as MapControl;
            var map = mapControl?.Map;

            if (map!.Navigator?.Viewport == null)
            {
                await Shell.Current.DisplayAlert("Error", "Unable to determine map position.", "OK");
                return;
            }

            // extract click location to map coordinates
            var screenPosition = e.ScreenPosition;
            var mapPosition = map.Navigator.Viewport.ScreenToWorld(screenPosition.X, screenPosition.Y);

            var result = await _popupService.ShowPopupAsync<CreatePlaceViewModel>();
            if (result is not CreatePlaceViewModel resultViewModel)
                return;

            MapModel.AddPointToMap(map, mapPosition);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error extracting tap location: {ex.Message}");
        }
        finally
        {
            e.Handled = true;
        }
    }

    public async void OnMapFeatureInfo(object? sender, MapInfoEventArgs e)
    {
        if (e.MapInfo?.Feature == null) return;

        // Only handle clicks on the points layer
        if (e.MapInfo.Layer!.Name == "user_points")
        {
            var feature = e.MapInfo.Feature;

            var pointId = feature["ID"] as String;
            if (pointId == null)
            {
                await Shell.Current.DisplayAlert("Error", "Point ID not found.", "OK");
                e.Handled = true;
                return;
            }

            var XY = pointId.Split(';');
            var x = double.Parse(XY[0]);
            var y = double.Parse(XY[1]);
            var position = new MPoint(x, y);

            await Shell.Current.GoToAsync(nameof(PlacePage));

            e.Handled = true;
        }
    }
}
