using CommunityToolkit.Maui.Views;

using WCecko.ViewModel;

namespace WCecko.View;

public partial class CreatePlacePopup : Popup
{
	public CreatePlacePopup(CreatePlaceViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}
