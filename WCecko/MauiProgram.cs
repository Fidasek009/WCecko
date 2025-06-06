using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
using WCecko.Model;
using WCecko.Model.Map;
using WCecko.Model.Rating;
using WCecko.Model.User;
using WCecko.View;
using WCecko.ViewModel;

namespace WCecko;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseSkiaSharp(true)
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("COMIC.TTF", "Comic Sans MS");
                fonts.AddFont("COMIC.TTF", "Comic Sans MS Bold");
            });

        // UI services
        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<LoginViewModel>();

        builder.Services.AddSingleton<RegisterPage>();
        builder.Services.AddSingleton<RegisterViewModel>();

        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<MainViewModel>();

        builder.Services.AddTransientPopup<CreatePlacePopup, CreatePlaceViewModel>();

        builder.Services.AddTransient<PlacePage>();
        builder.Services.AddTransient<PlaceViewModel>();

        builder.Services.AddTransientPopup<AddRatingPopup, AddRatingViewModel>();


        // Data services
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<UserService>();
        builder.Services.AddSingleton<MapService>();
        builder.Services.AddSingleton<RatingService>();


#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
