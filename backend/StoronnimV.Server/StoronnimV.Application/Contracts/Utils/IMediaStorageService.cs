using Microsoft.AspNetCore.Http;
using StoronnimV.Application.Enums;
using StoronnimV.Application.Models;

namespace StoronnimV.Application.Contracts.Utils;

public interface IMediaStorageService
{
    Task<StoredMedia> UploadAsync(
        IFormFile file,
        MediaKind mediaKind,
        string namePrefix,
        CancellationToken ct);

    Task<string> CreateAsync(
        IFormFile file,
        MediaKind mediaKind,
        string namePrefix,
        Func<string, Task> persistAsync,
        CancellationToken ct);

    Task<string> ReplaceAsync(
        IFormFile file,
        MediaKind mediaKind,
        string namePrefix,
        string? oldUrl,
        Func<string, Task> persistAsync,
        CancellationToken ct);

    Task DeleteAsync(
        MediaKind mediaKind,
        string? oldUrl,
        Func<Task> persistAsync,
        CancellationToken ct);

    Task DeleteByBlobNameAsync(
        MediaKind mediaKind,
        string blobName,
        Func<Task> persistAsync,
        CancellationToken ct);

    Task DeleteUploadedAsync(StoredMedia media, CancellationToken ct);
}
