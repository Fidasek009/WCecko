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

namespace WCecko.ViewModel;

[QueryProperty("Username", "Username")]
public partial class MainViewModel : ObservableObject
{
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
            // Extract the screen position from the event arguments
            var screenPosition = e.ScreenPosition;

            // Get the MapControl instance
            var mapControl = sender as MapControl;
            var map = mapControl?.Map;
            var pointsLayer = map!.Layers.FindLayer("user_points").First() as MemoryLayer;

            if (mapControl?.Map?.Navigator?.Viewport != null)
            {
                // Convert the screen position to map coordinates (MPoint)
                var mapPosition = mapControl.Map.Navigator.Viewport.ScreenToWorld(screenPosition.X, screenPosition.Y);
                var newPoint = MapModel.CreatePoint(mapPosition);
                var features = pointsLayer!.Features?.ToList() ?? new List<IFeature>();

                features.Add(newPoint);
                pointsLayer.Features = features;
                pointsLayer.DataHasChanged();
                mapControl.Map.Refresh();

                // Corrected the usage of ShowPopupAsync to pass an instance of CreatePlacePopup
                var popup = new CreatePlacePopup(new CreatePlaceViewModel());
                await Shell.Current.CurrentPage.ShowPopupAsync(popup);
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Unable to determine map position.", "OK");
            }
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

            // Run this on the UI thread since it might be called from a background thread
            MainThread.BeginInvokeOnMainThread(async () => {
                await Shell.Current.DisplayAlert(
                    "Point Clicked",
                    $"Point ID: {pointId}\nLocation: X={position.X:F2}, Y={position.Y:F2}",
                    "OK");

                // TODO: show place info
            });

            e.Handled = true;
        }
    }
}
