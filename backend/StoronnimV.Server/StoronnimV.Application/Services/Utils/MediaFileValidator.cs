using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using StoronnimV.Application.Contracts.Utils;
using StoronnimV.Application.Enums;
using StoronnimV.Application.Exceptions;
using StoronnimV.Application.Options;

namespace StoronnimV.Application.Services.Utils;

public sealed class MediaFileValidator(IOptions<MediaUploadOptions> options) : IMediaFileValidator
{
    private const int SignatureLength = 12;
    private readonly MediaUploadOptions _options = options.Value;

    public async Task ValidateAsync(IFormFile file, MediaKind mediaKind, CancellationToken ct)
    {
        if (file.Length == 0)
        {
            throw new MediaValidationException("Media file is empty.");
        }

        long maxBytes = mediaKind == MediaKind.Photo
            ? _options.MaxPhotoBytes
            : _options.MaxVideoBytes;

        if (file.Length > maxBytes)
        {
            throw new MediaValidationException($"Media file exceeds the configured {maxBytes}-byte limit.");
        }

        string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        string contentType = file.ContentType.Trim();
        string? expectedContentType = GetExpectedContentType(extension, mediaKind);
        string[] allowedContentTypes = mediaKind == MediaKind.Photo
            ? _options.AllowedPhotoContentTypes
            : _options.AllowedVideoContentTypes;

        if (expectedContentType is null
            || !allowedContentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase))
        {
            throw new MediaValidationException("Media file type is not allowed.");
        }

        if (!string.Equals(expectedContentType, contentType, StringComparison.OrdinalIgnoreCase))
        {
            throw new MediaValidationException("Media file extension and content type do not match.");
        }

        byte[] header = new byte[SignatureLength];
        await using Stream stream = file.OpenReadStream();
        int bytesRead = await stream.ReadAsync(header.AsMemory(0, SignatureLength), ct);

        if (!HasExpectedSignature(contentType, header.AsSpan(0, bytesRead)))
        {
            throw new MediaValidationException("Media file signature does not match its declared type.");
        }
    }

    private static string? GetExpectedContentType(string extension, MediaKind mediaKind)
    {
        return (mediaKind, extension) switch
        {
            (MediaKind.Photo, ".jpg" or ".jpeg") => "image/jpeg",
            (MediaKind.Photo, ".png") => "image/png",
            (MediaKind.Photo, ".webp") => "image/webp",
            (MediaKind.Video, ".mp4") => "video/mp4",
            _ => null
        };
    }

    private static bool HasExpectedSignature(string contentType, ReadOnlySpan<byte> header)
    {
        return contentType.ToLowerInvariant() switch
        {
            "image/jpeg" => header.Length >= 3
                            && header[0] == 0xFF
                            && header[1] == 0xD8
                            && header[2] == 0xFF,
            "image/png" => header.Length >= 8
                           && header[..8].SequenceEqual(new byte[]
                           {
                               0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A
                           }),
            "image/webp" => header.Length >= 12
                            && header[..4].SequenceEqual("RIFF"u8)
                            && header.Slice(8, 4).SequenceEqual("WEBP"u8),
            "video/mp4" => header.Length >= 8 && header.Slice(4, 4).SequenceEqual("ftyp"u8),
            _ => false
        };
    }
}
