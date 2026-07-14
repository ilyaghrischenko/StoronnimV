using System.Runtime.ExceptionServices;
using Microsoft.AspNetCore.Http;
using StoronnimV.Application.Contracts.Utils;
using StoronnimV.Application.Enums;
using StoronnimV.Application.Exceptions;
using StoronnimV.Application.Models;
using StoronnimV.Domain.Contracts.AzureBlobStorage;

namespace StoronnimV.Application.Services.Utils;

public sealed class MediaStorageService(
    IMediaFileValidator mediaFileValidator,
    IBlobRepository blobRepository) : IMediaStorageService
{
    public async Task<StoredMedia> UploadAsync(
        IFormFile file,
        MediaKind mediaKind,
        string namePrefix,
        CancellationToken ct)
    {
        await mediaFileValidator.ValidateAsync(file, mediaKind, ct);

        string containerName = GetContainerName(mediaKind);
        string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        string blobName = $"{namePrefix}-{Guid.NewGuid():N}{extension}";
        await using Stream fileStream = file.OpenReadStream();
        string url = await blobRepository.AddFileAndGetUrlAsync(
            containerName,
            blobName,
            fileStream,
            file.ContentType,
            ct);

        return new StoredMedia(containerName, blobName, url);
    }

    public async Task<string> CreateAsync(
        IFormFile file,
        MediaKind mediaKind,
        string namePrefix,
        Func<string, Task> persistAsync,
        CancellationToken ct)
    {
        StoredMedia uploaded = await UploadAsync(file, mediaKind, namePrefix, ct);

        try
        {
            await persistAsync(uploaded.Url);
        }
        catch (Exception exception)
        {
            await RollBackUploadAsync(uploaded, exception);
        }

        return uploaded.Url;
    }

    public async Task<string> ReplaceAsync(
        IFormFile file,
        MediaKind mediaKind,
        string namePrefix,
        string? oldUrl,
        Func<string, Task> persistAsync,
        CancellationToken ct)
    {
        string containerName = GetContainerName(mediaKind);
        string? oldBlobName = string.IsNullOrWhiteSpace(oldUrl)
            ? null
            : GetBlobName(oldUrl, containerName);
        StoredMedia uploaded = await UploadAsync(file, mediaKind, namePrefix, ct);

        try
        {
            await persistAsync(uploaded.Url);
        }
        catch (Exception exception)
        {
            await RollBackUploadAsync(uploaded, exception);
        }

        if (oldBlobName is not null)
        {
            await DeleteBlobAsync(containerName, oldBlobName, CancellationToken.None);
        }

        return uploaded.Url;
    }

    public async Task DeleteAsync(
        MediaKind mediaKind,
        string? oldUrl,
        Func<Task> persistAsync,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(oldUrl))
        {
            await persistAsync();
            return;
        }

        string containerName = GetContainerName(mediaKind);
        string blobName = GetBlobName(oldUrl, containerName);
        await persistAsync();
        await DeleteBlobAsync(containerName, blobName, CancellationToken.None);
    }

    public async Task DeleteByBlobNameAsync(
        MediaKind mediaKind,
        string blobName,
        Func<Task> persistAsync,
        CancellationToken ct)
    {
        await persistAsync();
        await DeleteBlobAsync(GetContainerName(mediaKind), blobName, CancellationToken.None);
    }

    public Task DeleteUploadedAsync(StoredMedia media, CancellationToken ct)
    {
        return DeleteBlobAsync(media.ContainerName, media.BlobName, ct);
    }

    private async Task RollBackUploadAsync(StoredMedia uploaded, Exception originalException)
    {
        try
        {
            await blobRepository.DeleteFileAsync(
                uploaded.ContainerName,
                uploaded.BlobName,
                CancellationToken.None);
        }
        catch (Exception cleanupException)
        {
            throw new MediaConsistencyException(
                uploaded.ContainerName,
                uploaded.BlobName,
                new AggregateException(originalException, cleanupException));
        }

        ExceptionDispatchInfo.Capture(originalException).Throw();
    }

    private async Task DeleteBlobAsync(string containerName, string blobName, CancellationToken ct)
    {
        try
        {
            await blobRepository.DeleteFileAsync(containerName, blobName, ct);
        }
        catch (Exception exception)
        {
            throw new MediaCleanupException(containerName, blobName, exception);
        }
    }

    private static string GetContainerName(MediaKind mediaKind)
    {
        return mediaKind == MediaKind.Photo ? "storonnimv-photo" : "storonnimv-video";
    }

    private static string GetBlobName(string url, string containerName)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
        {
            throw new MediaConsistencyException(
                containerName,
                "unknown",
                new FormatException("Stored media URL is not absolute."));
        }

        string[] segments = uri.AbsolutePath
            .Split('/', StringSplitOptions.RemoveEmptyEntries)
            .Select(Uri.UnescapeDataString)
            .ToArray();
        int containerIndex = Array.FindIndex(
            segments,
            segment => string.Equals(segment, containerName, StringComparison.Ordinal));

        if (containerIndex < 0 || containerIndex == segments.Length - 1)
        {
            throw new MediaConsistencyException(
                containerName,
                "unknown",
                new FormatException("Stored media URL does not contain the expected container and Blob name."));
        }

        return string.Join('/', segments[(containerIndex + 1)..]);
    }
}
