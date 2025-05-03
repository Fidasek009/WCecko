using Mapsui.Layers;
using Mapsui.Tiling;
using Mapsui.UI.Maui;
using WCecko.Model.Map;
using WCecko.ViewModel;

namespace WCecko;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel vm, MapService mapService)
    {
        InitializeComponent();
        BindingContext = vm;
        
        var mapControl = mapService.MapControl;

        mapControl.LongTap += vm.OnMapLongTapped;
        mapControl.Info += vm.OnMapFeatureInfo;

        MapContainer.Children.Add(mapControl);
    }
}
