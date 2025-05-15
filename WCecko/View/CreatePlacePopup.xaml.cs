namespace WCecko.View;

using CommunityToolkit.Maui.Views;

using WCecko.ViewModel;

public partial class CreatePlacePopup : Popup
{
    public CreatePlacePopup(CreatePlaceViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
