
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using WCecko.Model;
using WCecko.Model.Map;

namespace WCecko.ViewModel;

public partial class CreatePlaceViewModel(IPopupService popupService) : ObservableObject
{
    private readonly IPopupService popupService = popupService;


    [ObservableProperty]
    public partial string PlaceName { get; set; } = "";

    [ObservableProperty]
    public partial string PlaceDescription { get; set; } = "";

    [ObservableProperty]
    public partial ImageSource? PlaceImage { get; set; }


    [RelayCommand]
    async Task Cancel()
    {
        await popupService.ClosePopupAsync(null);
    }

    [RelayCommand]
    async Task Save()
    {
        await popupService.ClosePopupAsync(this);
    }

    [RelayCommand]
    async Task PickImage()
    {
        try
        {
            var image = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Select an image",
                FileTypes = FilePickerFileType.Images
            });
            if (image == null)
                return;

            using var stream = await image.OpenReadAsync();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            PlaceImage = ImageUtils.ResizeImageKeepAspectRatio(memoryStream, MapService.IMAGE_MAX_HEIGHT, MapService.IMAGE_MAX_WIDTH);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error picking image: {ex.Message}");
            return;
        }
    }

    [RelayCommand]
    async Task CaptureImage()
    {
        try
        {
            var photo = await MediaPicker.CapturePhotoAsync();
            if (photo == null)
                return;

            using var stream = await photo.OpenReadAsync();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            PlaceImage = ImageUtils.ResizeImageKeepAspectRatio(memoryStream, MapService.IMAGE_MAX_HEIGHT, MapService.IMAGE_MAX_WIDTH);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error capturing image: {ex.Message}");
            return;
        }
    }

    [RelayCommand]
    void RemoveImage()
    {
        PlaceImage = null;
    }
}
