
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mapsui;

namespace WCecko.ViewModel;

public partial class CreatePlaceViewModel : ObservableObject
{
    readonly IPopupService popupService;

    public CreatePlaceViewModel(IPopupService popupService)
    {
        this.popupService = popupService;
    }

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

    private byte[] ResizeImage(Stream inputStream, int width, int height)
    {
        using var originalBitmap = SkiaSharp.SKBitmap.Decode(inputStream);
        using var resizedBitmap = originalBitmap.Resize(new SkiaSharp.SKImageInfo(width, height), SkiaSharp.SKFilterQuality.Medium);
        using var image = SkiaSharp.SKImage.FromBitmap(resizedBitmap);
        return image.Encode(SkiaSharp.SKEncodedImageFormat.Jpeg, 80).ToArray(); // Compress to 80% quality
    }

    [RelayCommand]
    async Task PickImage()
    {
        try {
            var result = await FilePicker.PickAsync(new PickOptions {
                PickerTitle = "Select an image",
                FileTypes = FilePickerFileType.Images
            });

            if (result == null) {
                return;
            }

            using var stream = await result.OpenReadAsync();
            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);

            memoryStream.Position = 0;
            var resizedImage = ResizeImage(memoryStream, 200, 200); // Resize to 200x200 pixels
            PlaceImage = ImageSource.FromStream(() => new MemoryStream(resizedImage));
        }
        catch (Exception ex) {
            Console.WriteLine($"Error picking image: {ex.Message}");
            return;
        }
    }

    [RelayCommand]
    async Task CaptureImage()
    {
        try {
            var photo = await MediaPicker.CapturePhotoAsync();

            if (photo == null) {
                return;
            }

            using var stream = await photo.OpenReadAsync();
            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);

            memoryStream.Position = 0;
            var resizedImage = ResizeImage(memoryStream, 200, 200); // Resize to 200x200 pixels
            PlaceImage = ImageSource.FromStream(() => new MemoryStream(resizedImage));
        }
        catch (Exception ex) {
            Console.WriteLine($"Error capturing image: {ex.Message}");
            return;
        }
    }
}
