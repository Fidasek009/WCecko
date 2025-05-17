namespace WCecko.ViewModel;

using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using WCecko.Model;
using WCecko.Model.Map;


public partial class CreatePlaceViewModel(IPopupService popupService) : ObservableObject
{
    private readonly IPopupService _popupService = popupService;

    [ObservableProperty]
    public partial string Title { get; set; } = "Create new place";

    [ObservableProperty]
    public partial string PlaceName { get; set; } = "";

    [ObservableProperty]
    public partial string PlaceNameBorder { get; set; } = "Transparent";

    [ObservableProperty]
    public partial string PlaceDescription { get; set; } = "";

    [ObservableProperty]
    public partial ImageSource? PlaceImage { get; set; }


    partial void OnPlaceNameChanged(string value)
    {
        PlaceNameBorder = string.IsNullOrWhiteSpace(value) ? "Red" : "Transparent";
    }

    [RelayCommand]
    private async Task Cancel()
    {
        await _popupService.ClosePopupAsync(null);
    }

    [RelayCommand]
    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(PlaceName))
        {
            PlaceNameBorder = "Red";
            return;
        }

        await _popupService.ClosePopupAsync(this);
    }

    [RelayCommand]
    private async Task PickImage()
    {
        try
        {
            FileResult? image = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Select an image",
                FileTypes = FilePickerFileType.Images
            });
            if (image is null)
                return;

            MemoryStream? memoryStream = await ImageUtils.FileToStreamAsync(image);
            if (memoryStream is null || !ImageUtils.IsValidImage(memoryStream))
                return;

            PlaceImage = ImageUtils.ResizeImageKeepAspectRatio(memoryStream, Place.IMAGE_MAX_HEIGHT, Place.IMAGE_MAX_WIDTH);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error picking image: {ex.Message}");
            return;
        }
    }

    [RelayCommand]
    private async Task CaptureImage()
    {
        try
        {
            FileResult? photo = await MediaPicker.CapturePhotoAsync();
            if (photo is null)
                return;

            MemoryStream? memoryStream = await ImageUtils.FileToStreamAsync(photo);
            if (memoryStream is null || !ImageUtils.IsValidImage(memoryStream))
                return;

            PlaceImage = ImageUtils.ResizeImageKeepAspectRatio(memoryStream, Place.IMAGE_MAX_HEIGHT, Place.IMAGE_MAX_WIDTH);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error capturing image: {ex.Message}");
            return;
        }
    }

    [RelayCommand]
    private void RemoveImage()
    {
        PlaceImage = null;
    }
}
