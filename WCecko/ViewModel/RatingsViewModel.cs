using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using WCecko.Model;


namespace WCecko.ViewModel;

public partial class RatingsViewModel : ObservableObject
{
    [ObservableProperty]
    public partial double RatingMean { get; set; } = 4.8;

    [ObservableProperty]
    public partial double FiveStarPct { get; set; } = 0.6;

    [ObservableProperty]
    public partial double FourStarPct { get; set; } = 0.2;

    [ObservableProperty]
    public partial double ThreeStarPct { get; set; } = 0.07;

    [ObservableProperty]
    public partial double TwoStarPct { get; set; } = 0.03;

    [ObservableProperty]
    public partial double OneStarPct { get; set; } = 0.1;

    [ObservableProperty]
    public partial ObservableCollection<RatingModel> Ratings { get; set; } = [];

    [RelayCommand]
    async Task AddRating()
    {
        Ratings.Add(new RatingModel
        {
            UserName = "User" + (Ratings.Count + 1),
            Comment = "Great app!",
            Stars = 5,
        });
        // TODO
    }
}
