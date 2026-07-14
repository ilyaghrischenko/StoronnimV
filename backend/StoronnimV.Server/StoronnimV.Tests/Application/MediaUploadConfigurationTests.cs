using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;
using StoronnimV.Application.Options;

namespace StoronnimV.Tests.Application;

public sealed class MediaUploadConfigurationTests
{
    private const long TenMiB = 10L * 1024 * 1024;
    private const long TwoHundredFiftyMiB = 250L * 1024 * 1024;

    [Fact]
    public void AppSettings_ContainsConfirmedValidUploadPolicy()
    {
        // Arrange
        string testDirectory = Path.GetDirectoryName(GetSourceFilePath())!;
        string appSettingsPath = Path.GetFullPath(Path.Combine(
            testDirectory,
            "..",
            "..",
            "StoronnimV.Api",
            "appsettings.json"));
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddJsonFile(appSettingsPath)
            .Build();

        // Act
        MediaUploadOptions? options = configuration
            .GetRequiredSection(MediaUploadOptions.SectionName)
            .Get<MediaUploadOptions>();

        // Assert
        MediaUploadOptions policy = Assert.IsType<MediaUploadOptions>(options);
        Assert.True(policy.IsValid());
        Assert.Equal(TenMiB, policy.MaxPhotoBytes);
        Assert.Equal(TwoHundredFiftyMiB, policy.MaxVideoBytes);
        Assert.Equal(["image/jpeg", "image/png", "image/webp"], policy.AllowedPhotoContentTypes);
        Assert.Equal(["video/mp4"], policy.AllowedVideoContentTypes);
    }

    [Theory]
    [InlineData(0, TwoHundredFiftyMiB)]
    [InlineData(TenMiB, 0)]
    public void IsValid_NonPositiveLimit_RejectsPolicy(long photoLimit, long videoLimit)
    {
        // Arrange
        MediaUploadOptions options = new()
        {
            MaxPhotoBytes = photoLimit,
            MaxVideoBytes = videoLimit,
            AllowedPhotoContentTypes = ["image/jpeg"],
            AllowedVideoContentTypes = ["video/mp4"]
        };

        // Act
        bool isValid = options.IsValid();

        // Assert
        Assert.False(isValid);
    }

    [Theory]
    [InlineData(TenMiB + 1, TwoHundredFiftyMiB)]
    [InlineData(TenMiB, TwoHundredFiftyMiB + 1)]
    public void IsValid_LimitAboveConfirmedMaximum_RejectsPolicy(long photoLimit, long videoLimit)
    {
        // Arrange
        MediaUploadOptions options = new()
        {
            MaxPhotoBytes = photoLimit,
            MaxVideoBytes = videoLimit,
            AllowedPhotoContentTypes = ["image/jpeg"],
            AllowedVideoContentTypes = ["video/mp4"]
        };

        // Act
        bool isValid = options.IsValid();

        // Assert
        Assert.False(isValid);
    }

    [Theory]
    [InlineData("image/gif", "video/mp4")]
    [InlineData("image/jpeg", "video/quicktime")]
    public void IsValid_UnsupportedConfiguredType_RejectsPolicy(string photoType, string videoType)
    {
        // Arrange
        MediaUploadOptions options = new()
        {
            MaxPhotoBytes = TenMiB,
            MaxVideoBytes = TwoHundredFiftyMiB,
            AllowedPhotoContentTypes = [photoType],
            AllowedVideoContentTypes = [videoType]
        };

        // Act
        bool isValid = options.IsValid();

        // Assert
        Assert.False(isValid);
    }

    private static string GetSourceFilePath([CallerFilePath] string path = "") => path;
}
