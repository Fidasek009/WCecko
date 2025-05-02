using CommunityToolkit.Maui.Core;
using WCecko.ViewModel;

namespace WCecko.View;

public partial class RatingsView : ContentView
{
	public RatingsView()
	{
		InitializeComponent();

        if (IPlatformApplication.Current?.Services != null)
        {
            var popupService = IPlatformApplication.Current.Services.GetService<IPopupService>();
            if (popupService != null)
            {
                BindingContext = new RatingsViewModel(popupService);
            }
            else
            {
                // Fallback for design-time support
                Console.WriteLine("Warning: IPopupService not available.");
            }
        }
    }
}
