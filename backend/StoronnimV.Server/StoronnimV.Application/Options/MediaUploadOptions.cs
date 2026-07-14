namespace StoronnimV.Application.Options;

public sealed class MediaUploadOptions
{
    public const string SectionName = "MediaUpload";
    public const long MaxPhotoBytesLimit = 10L * 1024 * 1024;
    public const long MaxVideoBytesLimit = 250L * 1024 * 1024;

    private static readonly HashSet<string> SupportedPhotoContentTypes =
        new(StringComparer.OrdinalIgnoreCase) { "image/jpeg", "image/png", "image/webp" };

    private static readonly HashSet<string> SupportedVideoContentTypes =
        new(StringComparer.OrdinalIgnoreCase) { "video/mp4" };

    public long MaxPhotoBytes { get; init; }
    public long MaxVideoBytes { get; init; }
    public string[] AllowedPhotoContentTypes { get; init; } = [];
    public string[] AllowedVideoContentTypes { get; init; } = [];

    public bool IsValid()
    {
        return MaxPhotoBytes is > 0 and <= MaxPhotoBytesLimit
               && MaxVideoBytes is > 0 and <= MaxVideoBytesLimit
               && AllowedPhotoContentTypes.Length > 0
               && AllowedVideoContentTypes.Length > 0
               && AllowedPhotoContentTypes.All(SupportedPhotoContentTypes.Contains)
               && AllowedVideoContentTypes.All(SupportedVideoContentTypes.Contains);
    }
}
