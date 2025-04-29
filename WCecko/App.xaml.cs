using System.Runtime.Versioning;

namespace WCecko;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }
}
