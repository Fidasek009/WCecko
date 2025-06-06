namespace WCecko.ViewModel;

using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.UI.Maui;
using WCecko.Model.Map;
using WCecko.Model.User;
using WCecko.View;
using TappedEventArgs = Mapsui.UI.TappedEventArgs;


public partial class MainViewModel : ObservableObject
{
    private readonly IPopupService _popupService;
    private readonly UserService _userService;
    private readonly MapService _mapService;

    public MainViewModel(IPopupService popupService, UserService userService, MapService mapService)
    {
        _popupService = popupService;
        _userService = userService;
        _mapService = mapService;

        Username = _userService.CurrentUser?.Username ?? "Guest";
        _userService.UserChanged += OnUserChanged;
    }


    [ObservableProperty]
    public partial string Username { get; set; }


    [RelayCommand]
    private async Task Logout()
    {
        _userService.Logout();
        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }

    private void OnUserChanged(object? sender, User? e)
    {
        Username = e?.Username ?? "Guest";
    }

    public async void OnMapLongTapped(object? sender, TappedEventArgs e)
    {
        try
        {
            MapControl? mapControl = sender as MapControl;
            Map? map = mapControl?.Map;

            if (map!.Navigator?.Viewport is null)
            {
                await Shell.Current.DisplayAlert("Error", "Unable to determine map position.", "OK");
                return;
            }

            // extract click location to map coordinates
            MPoint screenPosition = e.ScreenPosition;
            MPoint mapPosition = map.Navigator.Viewport.ScreenToWorld(screenPosition.X, screenPosition.Y);

            object? popupResult = await _popupService.ShowPopupAsync<CreatePlaceViewModel>();
            if (popupResult is not CreatePlaceViewModel resultViewModel)
                return;

            var createResult = await _mapService.CreatePlaceAsync(mapPosition, resultViewModel.PlaceName, resultViewModel.PlaceDescription, resultViewModel.PlaceImage);
            if (!createResult)
            {
                await Shell.Current.DisplayAlert("Error", "Failed to create map point.", "OK");
                return;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in OnMapLongTapped: {ex.Message}");
            await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while creating the map point.", "OK");
        }
        finally
        {
            e.Handled = true;
        }
    }

    public async void OnMapFeatureInfo(object? sender, MapInfoEventArgs e)
    {
        try
        {
            if (e.MapInfo?.Feature is null)
                return;

            // Only handle clicks on the points layer
            if (e.MapInfo.Layer!.Name != MapService.POINTS_LAYER_NAME)
                return;

            IFeature feature = e.MapInfo.Feature;

            if (feature["ID"] is not int placeId)
            {
                await Shell.Current.DisplayAlert("Error", "Point ID not found.", "OK");
                return;
            }

            await Shell.Current.GoToAsync(nameof(PlacePage), new Dictionary<string, object>
                {
                    { "PlaceId", placeId }
                }
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in OnMapFeatureInfo: {ex.Message}");
            await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while displaying point information.", "OK");
        }
        finally
        {
            e.Handled = true;
        }
    }
}
