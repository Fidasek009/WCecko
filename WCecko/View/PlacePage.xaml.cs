using WCecko.ViewModel;

namespace WCecko.View;

public partial class PlacePage : ContentPage
{
	public PlacePage(PlaceViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
