namespace WCecko.View;

using CommunityToolkit.Maui.Views;

using WCecko.ViewModel;

public partial class AddRatingPopup : Popup
{
    public AddRatingPopup(AddRatingViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
