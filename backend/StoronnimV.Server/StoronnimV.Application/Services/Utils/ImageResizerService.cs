using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using StoronnimV.Application.Contracts.Utils;
using StoronnimV.Application.Exceptions;

namespace StoronnimV.Application.Services.Utils;

public class ImageResizerService : IImageResizerService
{
    private const int Quality = 85;

    public async Task<byte[]> ResizeImageByCompressIfNecessaryAsync(IFormFile photo, int width, int height,
        CancellationToken ct)
    {
        if (photo == null || photo.Length == 0)
            throw new PhotoResizingException("Photo cannot be null or empty.");

        // Открываем изображение
        await using var stream = photo.OpenReadStream();
        using var image = await Image.LoadAsync(stream, ct);

        // Проверяем, нужно ли делать ресайз
        if (image.Width <= width && image.Height <= height)
        {
            // Если размеры изображения уже соответствуют нужным, возвращаем исходное изображение
            return await ConvertImageToByteArrayAsync(image, ct);
        }

        // Если размеры не соответствуют, делаем ресайз
        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Mode = ResizeMode.Max, // Масштабирование без обрезки
            Size = new Size(width, height)
        }));

        // Сохраняем изображение в новый поток
        return await ConvertImageToByteArrayAsync(image, ct);
    }

    public async Task<byte[]> ResizeImageByCroppingIfNecessaryAsync(IFormFile photo, int width, int height,
        CancellationToken ct)
    {
        if (photo == null || photo.Length == 0)
            throw new PhotoResizingException("Photo cannot be null or empty.");

        // Открываем изображение
        await using var stream = photo.OpenReadStream();
        using var image = await Image.LoadAsync(stream, ct);

        // Проверяем, нужно ли делать ресайз
        if (image.Width <= width && image.Height <= height)
        {
            // Если размеры изображения уже соответствуют нужным, возвращаем исходное изображение
            return await ConvertImageToByteArrayAsync(image, ct);
        }

        // Если изображение больше, применяем ресайз с обрезкой по центру
        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Mode = ResizeMode.Crop, // Обрезка с сохранением пропорций
            Position = AnchorPositionMode.Center, // Фокус на центре
            Size = new Size(width, height)
        }));

        // Сохраняем изображение в новый поток
        return await ConvertImageToByteArrayAsync(image, ct);
    }

    private async Task<byte[]> ConvertImageToByteArrayAsync(Image image, CancellationToken ct)
    {
        await using var outputStream = new MemoryStream();
        await image.SaveAsync(outputStream, new JpegEncoder { Quality = Quality }, ct);
        return outputStream.ToArray();
    }
}