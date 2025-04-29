using Mapsui.Layers;
using Mapsui.Tiling;
using Mapsui.UI.Maui;
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

            mapControl.LongTap += vm.OnMapLongTapped;
            mapControl.Info += vm.OnMapFeatureInfo;

            return mapControl;
        }
    }
}
