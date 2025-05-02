using CommunityToolkit.Maui.Views;

using WCecko.ViewModel;

namespace WCecko.View;

public partial class AddRatingPopup : Popup
{
    public AddRatingPopup(AddRatingViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
