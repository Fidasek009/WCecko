namespace WCecko.View;

using WCecko.ViewModel;

public partial class PlacePage : ContentPage
{
    public PlacePage(PlaceViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
