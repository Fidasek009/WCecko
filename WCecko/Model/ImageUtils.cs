namespace WCecko.Model;

using Mapsui.Styles;


public static class ImageUtils
{
    public static readonly string IMAGE_DIR = Path.Combine(FileSystem.AppDataDirectory, "images");

    public static ImageSource ResizeImageKeepAspectRatio(Stream inputStream, int maxWidth, int maxHeight)
    {
        inputStream.Position = 0;
        using SkiaSharp.SKBitmap originalBitmap = SkiaSharp.SKBitmap.Decode(inputStream);

        int origWidth = originalBitmap.Width;
        int origHeight = originalBitmap.Height;

        // Calculate the new dimensions while keeping the aspect ratio
        float ratioX = (float)maxWidth / origWidth;
        float ratioY = (float)maxHeight / origHeight;
        float ratio = Math.Min(ratioX, ratioY);

        int newWidth = (int)(origWidth * ratio);
        int newHeight = (int)(origHeight * ratio);

        using SkiaSharp.SKBitmap resizedBitmap = originalBitmap.Resize(new SkiaSharp.SKImageInfo(newWidth, newHeight), SkiaSharp.SKFilterQuality.High);
        using SkiaSharp.SKImage image = SkiaSharp.SKImage.FromBitmap(resizedBitmap);
        byte[] resizedImage = image.Encode(SkiaSharp.SKEncodedImageFormat.Jpeg, 80).ToArray(); // Compress to 80% quality

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

        using Stream stream = await imageStream.Stream(CancellationToken.None);
        using FileStream fileStream = File.Create(imagePath);
        await stream.CopyToAsync(fileStream);

        return imagePath;
    }

    public static int RegisterBitmapAsync(string imageName)
    {
        try
        {
            string resourceName = $"WCecko.Resources.Images.{imageName}";
            System.Reflection.Assembly assembly = typeof(ImageUtils).Assembly;

            Stream? stream = assembly.GetManifestResourceStream(resourceName);
            if (stream is null)
            {
                Console.WriteLine($"Failed to load embedded resource: {resourceName}");
                return -1;
            }

            return BitmapRegistry.Instance.Register(stream);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error registering bitmap: {ex.Message}");
            return -1;
        }
    }
}
