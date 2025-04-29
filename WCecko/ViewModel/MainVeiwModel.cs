using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Styles;
using Mapsui.Tiling;
using Mapsui.Widgets;
using Mapsui.UI.Maui;
using Mapsui.Extensions;

using TappedEventArgs = Mapsui.UI.TappedEventArgs;

using WCecko.Model;


namespace WCecko.ViewModel
{
    [QueryProperty("Username", "Username")]
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        string _username = "";

        [RelayCommand]
        async Task Logout()
        {
            await Shell.Current.GoToAsync("..");
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
                var pointsLayer = map.Layers.FindLayer("user_points").First() as MemoryLayer;

                if (mapControl?.Map?.Navigator?.Viewport != null)
                {
                    // Convert the screen position to map coordinates (MPoint)
                    var mapPosition = mapControl.Map.Navigator.Viewport.ScreenToWorld(screenPosition.X, screenPosition.Y);

                    // Display the map coordinates
                    await Shell.Current.DisplayAlert("Tap Location", $"Map Coordinates: X = {mapPosition.X}, Y = {mapPosition.Y}", "OK");

                    var feature = MapModel.CreatePoint(mapPosition);
                    var features = pointsLayer.Features?.ToList() ?? new List<IFeature>();
                    features.Add(feature);
                    pointsLayer.Features = features;
                    pointsLayer.DataHasChanged();
                    mapControl.Map.Refresh();
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
        }
    }
}
