using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using Mapsui;
using Mapsui.Layers;
using Mapsui.UI.Maui;
using Mapsui.Extensions;
using CommunityToolkit.Maui.Views;

using TappedEventArgs = Mapsui.UI.TappedEventArgs;

using WCecko.Model;
using WCecko.View;
using CommunityToolkit.Maui.Core;
using System.Collections.Generic;

namespace WCecko.ViewModel;

[QueryProperty("Username", "Username")]
public partial class MainViewModel : ObservableObject
{
    private readonly IPopupService popupService;

    public MainViewModel(IPopupService popupService)
    {
        this.popupService = popupService;
    }

    [ObservableProperty]
    public partial string Username { get; set; } = "";

    [RelayCommand]
    async Task Logout()
    {
        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }

    public async void OnMapLongTapped(object? sender, TappedEventArgs e)
    {
        try
        {
            var mapControl = sender as MapControl;
            var map = mapControl?.Map;

            if (map!.Navigator?.Viewport == null) {
                await Shell.Current.DisplayAlert("Error", "Unable to determine map position.", "OK");
                return;
            }

            // extract click location to map coordinates
            var screenPosition = e.ScreenPosition;
            var mapPosition = map.Navigator.Viewport.ScreenToWorld(screenPosition.X, screenPosition.Y);

            var result = await popupService.ShowPopupAsync<CreatePlaceViewModel>();
            if (result is not CreatePlaceViewModel resultViewModel) {
                return;
            }

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
            if (pointId == null) {
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
