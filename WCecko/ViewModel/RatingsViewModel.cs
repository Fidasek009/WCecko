using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;

using WCecko.Model.Rating;

namespace WCecko.ViewModel;

public partial class RatingsViewModel : ObservableObject
{
    private readonly IPopupService _popupService;

    public RatingsViewModel(IPopupService popupService)
    {
        _popupService = popupService;
        Ratings.CollectionChanged += UpdateRatingStats;
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
    public partial ObservableCollection<Rating> Ratings { get; set; } = [];


    partial void OnRatingsChanged(ObservableCollection<Rating> value)
    {
        value.CollectionChanged += UpdateRatingStats;
    }

    private void UpdateRatingStats(object? sendser, NotifyCollectionChangedEventArgs e)
    {
        if (Ratings.Count == 0)
            return;

        int[] starCounts = new int[5];
        int totalStars = 0;

        foreach (var r in Ratings)
        {
            starCounts[r.Stars - 1]++;
            totalStars += r.Stars;
        }

        FiveStarPct = starCounts[4] / (double)Ratings.Count;
        FourStarPct = starCounts[3] / (double)Ratings.Count;
        ThreeStarPct = starCounts[2] / (double)Ratings.Count;
        TwoStarPct = starCounts[1] / (double)Ratings.Count;
        OneStarPct = starCounts[0] / (double)Ratings.Count;

        RatingMean = Math.Round(totalStars / (double)Ratings.Count, 1);
    }


    [RelayCommand]
    async Task AddRating()
    {
        try
        {
            var result = await _popupService.ShowPopupAsync<AddRatingViewModel>();
            if (result is not AddRatingViewModel resultViewModel)
                return;
            
            Ratings.Add(new Rating
            {
                Username = "User" + (Ratings.Count + 1),
                Comment = resultViewModel.Comment,
                Stars = resultViewModel.Stars,
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error adding rating: {ex.Message}");
        }
    }

    [RelayCommand]
    async Task DeleteRating(Rating rating)
    {
        try
        {
            var result = await Shell.Current.DisplayAlert(
                "Delete Rating",
                "Are you sure you want to delete this rating?",
                "Yes",
                "No");

            if (!result)
                return;

            Ratings.Remove(rating);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting rating: {ex.Message}");
        }
    }

    [RelayCommand]
    async Task EditRating(Rating rating)
    {
        // TODO
    }
}
