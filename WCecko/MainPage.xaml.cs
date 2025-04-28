using Mapsui.Tiling;
using Mapsui.UI.Maui;
using Mapsui.Widgets;

namespace WCecko
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            var mapControl = new MapControl();
            var map = mapControl.Map;

            map.Layers.Add(OpenStreetMap.CreateTileLayer());

            Content = mapControl;
        }
    }
}
