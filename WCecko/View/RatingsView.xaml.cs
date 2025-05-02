using WCecko.ViewModel;

namespace WCecko.View;

public partial class RatingsView : ContentView
{
	public RatingsView()
	{
		InitializeComponent();
		BindingContext = new RatingsViewModel();
	}
}
