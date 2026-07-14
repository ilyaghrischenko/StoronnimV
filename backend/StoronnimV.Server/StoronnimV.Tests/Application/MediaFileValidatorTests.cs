using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using StoronnimV.Application.Enums;
using StoronnimV.Application.Exceptions;
using StoronnimV.Application.Options;
using StoronnimV.Application.Services.Utils;

namespace StoronnimV.Tests.Application;

public sealed class MediaFileValidatorTests
{
    private const long TenMiB = 10L * 1024 * 1024;
    private const long TwoHundredFiftyMiB = 250L * 1024 * 1024;

    [Theory]
    [MemberData(nameof(ValidPhotoFiles))]
    public async Task ValidateAsync_AllowedPhotoAtLimit_AcceptsFile(
        string fileName,
        string contentType,
        byte[] signature)
    {
        // Arrange
        MediaFileValidator validator = CreateValidator();
        IFormFile file = CreateFile(fileName, contentType, signature, TenMiB);

        // Act
        Exception? exception = await Record.ExceptionAsync(
            () => validator.ValidateAsync(file, MediaKind.Photo, CancellationToken.None));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public async Task ValidateAsync_AllowedMp4AtLimit_AcceptsFile()
    {
        // Arrange
        MediaFileValidator validator = CreateValidator();
        IFormFile file = CreateFile("clip.mp4", "video/mp4", Mp4Signature(), TwoHundredFiftyMiB);

        // Act
        Exception? exception = await Record.ExceptionAsync(
            () => validator.ValidateAsync(file, MediaKind.Video, CancellationToken.None));

        // Assert
        Assert.Null(exception);
    }

    [Theory]
    [InlineData(MediaKind.Photo, "photo.jpg", "image/jpeg", TenMiB + 1)]
    [InlineData(MediaKind.Video, "clip.mp4", "video/mp4", TwoHundredFiftyMiB + 1)]
    public async Task ValidateAsync_FileAboveConfiguredLimit_RejectsFile(
        MediaKind mediaKind,
        string fileName,
        string contentType,
        long length)
    {
        // Arrange
        MediaFileValidator validator = CreateValidator();
        byte[] signature = mediaKind == MediaKind.Photo ? JpegSignature() : Mp4Signature();
        IFormFile file = CreateFile(fileName, contentType, signature, length);

        // Act
        MediaValidationException exception = await Assert.ThrowsAsync<MediaValidationException>(
            () => validator.ValidateAsync(file, mediaKind, CancellationToken.None));

        // Assert
        Assert.Contains("limit", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ValidateAsync_EmptyFile_RejectsFile()
    {
        // Arrange
        MediaFileValidator validator = CreateValidator();
        IFormFile file = CreateFile("photo.jpg", "image/jpeg", JpegSignature(), 0);

        // Act
        MediaValidationException exception = await Assert.ThrowsAsync<MediaValidationException>(
            () => validator.ValidateAsync(file, MediaKind.Photo, CancellationToken.None));

        // Assert
        Assert.Contains("empty", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("photo.gif", "image/gif")]
    [InlineData("photo.exe", "application/octet-stream")]
    public async Task ValidateAsync_UnsupportedPhotoType_RejectsFile(string fileName, string contentType)
    {
        // Arrange
        MediaFileValidator validator = CreateValidator();
        IFormFile file = CreateFile(fileName, contentType, [0x47, 0x49, 0x46, 0x38], 4);

        // Act
        MediaValidationException exception = await Assert.ThrowsAsync<MediaValidationException>(
            () => validator.ValidateAsync(file, MediaKind.Photo, CancellationToken.None));

        // Assert
        Assert.Contains("type", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("photo.png", "image/jpeg")]
    [InlineData("photo.jpg", "image/png")]
    public async Task ValidateAsync_ExtensionAndMimeDoNotMatch_RejectsFile(string fileName, string contentType)
    {
        // Arrange
        MediaFileValidator validator = CreateValidator();
        MediaKind mediaKind = contentType == "video/mp4" ? MediaKind.Video : MediaKind.Photo;
        byte[] signature = mediaKind == MediaKind.Video ? Mp4Signature() : JpegSignature();
        IFormFile file = CreateFile(fileName, contentType, signature, signature.Length);

        // Act
        MediaValidationException exception = await Assert.ThrowsAsync<MediaValidationException>(
            () => validator.ValidateAsync(file, mediaKind, CancellationToken.None));

        // Assert
        Assert.Contains("match", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(MediaKind.Photo, "photo.jpg", "image/jpeg")]
    [InlineData(MediaKind.Photo, "photo.png", "image/png")]
    [InlineData(MediaKind.Photo, "photo.webp", "image/webp")]
    [InlineData(MediaKind.Video, "clip.mp4", "video/mp4")]
    public async Task ValidateAsync_InvalidSignature_RejectsFile(
        MediaKind mediaKind,
        string fileName,
        string contentType)
    {
        // Arrange
        MediaFileValidator validator = CreateValidator();
        byte[] invalidSignature = [0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B];
        IFormFile file = CreateFile(fileName, contentType, invalidSignature, invalidSignature.Length);

        // Act
        MediaValidationException exception = await Assert.ThrowsAsync<MediaValidationException>(
            () => validator.ValidateAsync(file, mediaKind, CancellationToken.None));

        // Assert
        Assert.Contains("signature", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    public static TheoryData<string, string, byte[]> ValidPhotoFiles => new()
    {
        { "photo.jpg", "image/jpeg", JpegSignature() },
        { "photo.png", "image/png", PngSignature() },
        { "photo.webp", "image/webp", WebpSignature() }
    };

    private static MediaFileValidator CreateValidator()
    {
        MediaUploadOptions options = new()
        {
            MaxPhotoBytes = TenMiB,
            MaxVideoBytes = TwoHundredFiftyMiB,
            AllowedPhotoContentTypes = ["image/jpeg", "image/png", "image/webp"],
            AllowedVideoContentTypes = ["video/mp4"]
        };

        return new MediaFileValidator(Options.Create(options));
    }

    private static IFormFile CreateFile(string fileName, string contentType, byte[] bytes, long length)
    {
        MemoryStream stream = new(bytes);
        return new FormFile(stream, 0, length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }

    private static byte[] JpegSignature() => [0xFF, 0xD8, 0xFF, 0xE0];

    private static byte[] PngSignature() => [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

    private static byte[] WebpSignature() =>
        [0x52, 0x49, 0x46, 0x46, 0x04, 0x00, 0x00, 0x00, 0x57, 0x45, 0x42, 0x50];

    private static byte[] Mp4Signature() =>
        [0x00, 0x00, 0x00, 0x18, 0x66, 0x74, 0x79, 0x70, 0x69, 0x73, 0x6F, 0x6D];
}
