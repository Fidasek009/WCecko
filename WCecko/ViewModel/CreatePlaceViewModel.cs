
using CommunityToolkit.Mvvm.ComponentModel;

namespace WCecko.ViewModel;

public partial class CreatePlaceViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string PlaceName { get; set; } = "";

    [ObservableProperty]
    public partial string PlaceDescription { get; set; } = "";

    //[ObservableProperty]
    //public partial string PlaceImage { get; set; } = "";
}
