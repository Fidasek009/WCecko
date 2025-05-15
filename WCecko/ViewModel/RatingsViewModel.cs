namespace WCecko.ViewModel;

using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using WCecko.Model.Rating;


public partial class RatingsViewModel : ObservableObject
{
    private readonly IPopupService _popupService;
    private readonly RatingService _ratingService;

    public RatingsViewModel(IPopupService popupService, RatingService ratingService)
    {
        _popupService = popupService;
        _ratingService = ratingService;
        Ratings.CollectionChanged += OnRatingsCollectionChanged;
        Task.Run(LoadRatings);
    }

    [ObservableProperty]
    public partial int PlaceId { get; set; }

    [ObservableProperty]
    public partial double RatingMean { get; set; } = 0.0;

    [ObservableProperty]
    public partial double FiveStarPct { get; set; } = 0.0;

    [ObservableProperty]
    public partial double FourStarPct { get; set; } = 0.0;

    [ObservableProperty]
    public partial double ThreeStarPct { get; set; } = 0.0;

    [ObservableProperty]
    public partial double TwoStarPct { get; set; } = 0.0;

    [ObservableProperty]
    public partial double OneStarPct { get; set; } = 0.0;

    [ObservableProperty]
    public partial ObservableCollection<Rating> Ratings { get; set; } = [];


    partial void OnPlaceIdChanged(int value)
    {
        Task.Run(LoadRatings);
    }

    partial void OnRatingsChanged(ObservableCollection<Rating> value)
    {
        value.CollectionChanged += OnRatingsCollectionChanged;
    }

    private void OnRatingsCollectionChanged(object? sendser, NotifyCollectionChangedEventArgs e)
    {
        UpdateRatingStats();
    }

    private void UpdateRatingStats()
    {
        if (Ratings.Count == 0)
        {
            FiveStarPct = 0;
            FourStarPct = 0;
            ThreeStarPct = 0;
            TwoStarPct = 0;
            OneStarPct = 0;
            RatingMean = 0;
            return;
        }

        int[] starCounts = new int[5];
        int totalStars = 0;

        foreach (Rating r in Ratings)
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


    private async Task LoadRatings()
    {
        IReadOnlyList<Rating> ratings = await _ratingService.GetPlaceRatingsAsync(PlaceId);
        if (ratings is null)
            return;
        Ratings = [.. ratings];
        UpdateRatingStats();
    }


    [RelayCommand]
    async Task AddRating()
    {
        object? result = await _popupService.ShowPopupAsync<AddRatingViewModel>();
        if (result is not AddRatingViewModel resultViewModel)
            return;

        var rating = await _ratingService.CreateRatingAsync(PlaceId, resultViewModel.Stars, resultViewModel.Comment);
        if (rating is null)
            return;

        Ratings.Add(rating);
    }

    [RelayCommand]
    async Task DeleteRating(Rating rating)
    {
        bool confirm = await Shell.Current.DisplayAlert("Delete Rating", "Are you sure you want to delete this rating?", "Yes", "No");
        if (!confirm)
            return;

        bool deleteResult = await _ratingService.DeleteRatingAsync(rating);
        if (!deleteResult)
        {
            await Shell.Current.DisplayAlert("Error", "Failed to delete rating.", "OK");
            return;
        }

        Ratings.Remove(rating);
    }

    [RelayCommand]
    async Task EditRating(Rating rating)
    {
        var editResult = await _popupService.ShowPopupAsync<AddRatingViewModel>(
            onPresenting: vm =>
            {
                vm.Title = "Edit rating";
                vm.Stars = rating.Stars;
                vm.Comment = rating.Comment;
            });
        if (editResult is not AddRatingViewModel resultViewModel)
            return;

        int prevStars = rating.Stars;
        string prevComment = rating.Comment;

        rating.Stars = resultViewModel.Stars;
        rating.Comment = resultViewModel.Comment;

        bool updateResult = await _ratingService.UpdateRatingAsync(rating);
        if (!updateResult)
        {
            rating.Stars = prevStars;
            rating.Comment = prevComment;
            await Shell.Current.DisplayAlert("Error", "Failed to update rating.", "OK");
            return;
        }

        if (prevStars != rating.Stars)
            UpdateRatingStats();
    }
}
