namespace WCecko;

using WCecko.Model.Map;
using WCecko.ViewModel;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel vm, MapService mapService)
    {
        InitializeComponent();
        BindingContext = vm;

        Mapsui.UI.Maui.MapControl mapControl = mapService.MapControl;

        mapControl.LongTap += vm.OnMapLongTapped;
        mapControl.Info += vm.OnMapFeatureInfo;

        MapContainer.Children.Add(mapControl);
    }
}
