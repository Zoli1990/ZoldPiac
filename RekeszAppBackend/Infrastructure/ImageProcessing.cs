using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace RekeszAppBackend.Infrastructure;

// A feltöltött fotókat tárolás előtt webes megjelenítéshez megfelelő méretre és minőségre
// alakítjuk. Az eredeti több megabájtos mobilfotót soha nem tároljuk el.
public static class ImageProcessing
{
    private const int MaxDimension = 640;
    private const int JpegQuality = 75;

    public static async Task SaveResizedAsync(Stream input, string destinationPath)
    {
        using var image = await Image.LoadAsync(input);
        if (image.Width > MaxDimension || image.Height > MaxDimension)
        {
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(MaxDimension, MaxDimension)
            }));
        }

        var encoder = new JpegEncoder { Quality = JpegQuality };
        await image.SaveAsync(destinationPath, encoder);
    }
}
