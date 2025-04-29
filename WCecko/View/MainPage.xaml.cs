using Mapsui.Extensions;
using Mapsui.Layers;
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
            MapContainer.Children.Add(CreateMap(vm));
        }

        private static MapControl CreateMap(MainViewModel vm)
        {
            var mapControl = new MapControl();
            var map = mapControl.Map;
            var pointsLayer = new MemoryLayer
            {
                Name = "user_points",
                Style = null,
                IsMapInfoLayer = true
            };

            map.Layers.Add(OpenStreetMap.CreateTileLayer());
            map.Layers.Add(pointsLayer);
            map.Widgets.Add(new MapInfoWidget(map));

            mapControl.LongTap += vm.OnMapLongTapped;

            return mapControl;
        }
    }
}
