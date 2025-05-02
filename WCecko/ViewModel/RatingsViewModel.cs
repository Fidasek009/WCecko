using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using WCecko.Model;
using WCecko.View;


namespace WCecko.ViewModel;

public partial class RatingsViewModel : ObservableObject
{
    private readonly IPopupService _popupService;

    public RatingsViewModel(IPopupService popupService)
    {
        _popupService = popupService;
    }

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
        try
        {
            var result = await _popupService.ShowPopupAsync<AddRatingViewModel>();
            if (result is not AddRatingViewModel resultViewModel)
                return;
            
            Ratings.Add(new RatingModel
            {
                UserName = "User" + (Ratings.Count + 1),
                Comment = resultViewModel.Comment,
                Stars = resultViewModel.Stars,
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error adding rating: {ex.Message}");
        }
    }
}
