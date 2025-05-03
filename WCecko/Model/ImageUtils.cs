
namespace WCecko.Model;

public static class ImageUtils
{
    public static readonly string IMAGE_DIR = Path.Combine(FileSystem.AppDataDirectory, "images");

    public static ImageSource ResizeImageKeepAspectRatio(Stream inputStream, int maxWidth, int maxHeight)
    {
        inputStream.Position = 0;
        using var originalBitmap = SkiaSharp.SKBitmap.Decode(inputStream);

        int origWidth = originalBitmap.Width;
        int origHeight = originalBitmap.Height;

        // Calculate the new dimensions while keeping the aspect ratio
        float ratioX = (float)maxWidth / origWidth;
        float ratioY = (float)maxHeight / origHeight;
        float ratio = Math.Min(ratioX, ratioY);

        int newWidth = (int)(origWidth * ratio);
        int newHeight = (int)(origHeight * ratio);

        using var resizedBitmap = originalBitmap.Resize(new SkiaSharp.SKImageInfo(newWidth, newHeight), SkiaSharp.SKFilterQuality.High);
        using var image = SkiaSharp.SKImage.FromBitmap(resizedBitmap);
        var resizedImage = image.Encode(SkiaSharp.SKEncodedImageFormat.Jpeg, 80).ToArray(); // Compress to 80% quality

        return ImageSource.FromStream(() => new MemoryStream(resizedImage));
    }

    public static async Task<string> SaveImageAsync(ImageSource image, string filename)
    {
        // image is already saved to file
        if (image is FileImageSource fileImageSource)
        {
            return fileImageSource.File;
        }
        if (image is not StreamImageSource imageStream)
        {
            throw new ArgumentException("Image must be a FileImageSource or StreamImageSource");
        }

        Directory.CreateDirectory(IMAGE_DIR);
        string imagePath = Path.Combine(IMAGE_DIR, filename);

        using var stream = await imageStream.Stream(CancellationToken.None);
        using var fileStream = File.Create(imagePath);
        await stream.CopyToAsync(fileStream);

        return imagePath;
    }
}
