using Mapsui.Extensions;
using Mapsui.Tiling;
using Mapsui.UI.Maui;
using Mapsui.Widgets;
using WCecko.ViewModel;

namespace WCecko
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;

            var mapControl = new MapControl();
            var map = mapControl.Map;

            map.Layers.Add(OpenStreetMap.CreateTileLayer());
            map.Widgets.Add(new MapInfoWidget(map));

            Content = mapControl;
        }
    }
}
