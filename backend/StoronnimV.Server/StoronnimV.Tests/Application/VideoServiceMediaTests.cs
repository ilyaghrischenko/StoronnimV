using Microsoft.AspNetCore.Http;
using StoronnimV.Application.Contracts.Utils;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Addition;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing;
using StoronnimV.Application.Enums;
using StoronnimV.Application.Exceptions;
using StoronnimV.Application.Models;
using StoronnimV.Application.Services.Entities;
using StoronnimV.Domain.Contracts.Database;
using StoronnimV.Domain.Entities;
using StoronnimV.Domain.Enums;
using StoronnimV.Domain.Projections.Video;

namespace StoronnimV.Tests.Application;

public sealed class VideoServiceMediaTests
{
    [Fact]
    public async Task AddPromotionAsync_UploadFails_PreservesOldPromotion()
    {
        // Arrange
        RecordingVideoRepository repository = CreateRepositoryWithPromotion();
        RecordingMediaStorageService mediaStorage = new() { FailUpload = true };
        VideoService service = new(repository, mediaStorage);

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.AddVideoAsync(CreateRequest(), CancellationToken.None));

        // Assert
        Assert.Single(repository.Videos);
        Assert.Equal("old-promotion.mp4", repository.Videos[0].BlobName);
        Assert.Equal(["upload"], mediaStorage.Operations);
    }

    [Fact]
    public async Task AddVideoAsync_NewDatabaseInsertFails_DeletesNewBlobAndPreservesExistingPromotion()
    {
        // Arrange
        RecordingVideoRepository repository = CreateRepositoryWithPromotion();
        repository.FailAdd = true;
        RecordingMediaStorageService mediaStorage = new() { Timeline = repository.Operations };
        VideoService service = new(repository, mediaStorage);

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.AddVideoAsync(CreateRequest(VideoType.Performance), CancellationToken.None));

        // Assert
        Assert.Single(repository.Videos);
        Assert.Equal("old-promotion.mp4", repository.Videos[0].BlobName);
        Assert.Equal(["upload", "delete-new-blob"], mediaStorage.Operations);
    }

    [Fact]
    public async Task AddPromotionAsync_AtomicDatabaseReplaceFails_DeletesNewBlobAndPreservesOldPromotion()
    {
        // Arrange
        RecordingVideoRepository repository = CreateRepositoryWithPromotion();
        repository.FailReplace = true;
        RecordingMediaStorageService mediaStorage = new();
        VideoService service = new(repository, mediaStorage);

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.AddVideoAsync(CreateRequest(), CancellationToken.None));

        // Assert
        Video remaining = Assert.Single(repository.Videos);
        Assert.Equal(1, remaining.Id);
        Assert.Equal("old-promotion.mp4", remaining.BlobName);
        Assert.Equal(
            ["upload", "delete-new-blob"],
            mediaStorage.Operations);
    }

    [Fact]
    public async Task AddPromotionAsync_OldBlobCleanupFails_KeepsNewPromotionAndReportsOrphan()
    {
        // Arrange
        RecordingVideoRepository repository = CreateRepositoryWithPromotion();
        RecordingMediaStorageService mediaStorage = new() { FailOldBlobDelete = true };
        VideoService service = new(repository, mediaStorage);

        // Act
        MediaCleanupException exception = await Assert.ThrowsAsync<MediaCleanupException>(
            () => service.AddVideoAsync(CreateRequest(), CancellationToken.None));

        // Assert
        Video remaining = Assert.Single(repository.Videos);
        Assert.Equal("video-new.mp4", remaining.BlobName);
        Assert.Equal("old-promotion.mp4", exception.BlobName);
        Assert.Equal(["upload", "delete-old-promotion", "delete-blob:old-promotion.mp4"], mediaStorage.Operations);
    }

    [Fact]
    public async Task AddPromotionAsync_Succeeds_AddsNewBeforeDeletingOldPromotion()
    {
        // Arrange
        RecordingVideoRepository repository = CreateRepositoryWithPromotion();
        RecordingMediaStorageService mediaStorage = new() { Timeline = repository.Operations };
        VideoService service = new(repository, mediaStorage);

        // Act
        await service.AddVideoAsync(CreateRequest(), CancellationToken.None);

        // Assert
        Video remaining = Assert.Single(repository.Videos);
        Assert.Equal("video-new.mp4", remaining.BlobName);
        Assert.Equal(
            ["upload", "database-replace", "delete-old-promotion", "delete-blob:old-promotion.mp4"],
            repository.CombinedOperations(mediaStorage));
    }

    [Fact]
    public async Task UpdateVideoAsync_ChangingCategoryToPromotion_IsRejected()
    {
        RecordingVideoRepository repository = new();
        repository.Videos.Add(new Video
        {
            Id = 1,
            Title = "performance",
            Url = "https://storage.test/storonnimv-video/performance.mp4",
            BlobName = "performance.mp4",
            Type = VideoType.Performance
        });
        VideoService service = new(repository, new RecordingMediaStorageService());

        await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateVideoAsync(
            new VideoEditRequest { Id = 1, Title = "promotion", Type = "Promotion" },
            CancellationToken.None));

        Assert.Equal(VideoType.Performance, repository.Videos[0].Type);
    }

    [Fact]
    public async Task UpdateVideoAsync_ChangingPromotionToCategory_IsRejected()
    {
        RecordingVideoRepository repository = CreateRepositoryWithPromotion();
        VideoService service = new(repository, new RecordingMediaStorageService());

        await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateVideoAsync(
            new VideoEditRequest { Id = 1, Title = "performance", Type = "Performance" },
            CancellationToken.None));

        Assert.Equal(VideoType.Promotion, repository.Videos[0].Type);
    }

    private static RecordingVideoRepository CreateRepositoryWithPromotion()
    {
        RecordingVideoRepository repository = new();
        repository.Videos.Add(new Video
        {
            Id = 1,
            Title = "old",
            Url = "https://storage.test/storonnimv-video/old-promotion.mp4",
            BlobName = "old-promotion.mp4",
            Type = VideoType.Promotion
        });
        return repository;
    }

    private static VideoAdditionRequest CreateRequest(VideoType type = VideoType.Promotion)
    {
        MemoryStream stream = new([0x00, 0x00, 0x00, 0x18, 0x66, 0x74, 0x79, 0x70]);
        IFormFile file = new FormFile(stream, 0, stream.Length, "url", "promotion.mp4")
        {
            Headers = new HeaderDictionary(),
            ContentType = "video/mp4"
        };

        return new VideoAdditionRequest
        {
            Url = file,
            Title = "new",
            Type = type.ToString()
        };
    }

    private sealed class RecordingVideoRepository : IVideoRepository
    {
        public List<Video> Videos { get; } = [];
        public List<string> Operations { get; } = [];
        public bool FailAdd { get; set; }
        public bool FailReplace { get; set; }

        public Task<Video?> GetByIdAsync(long id, CancellationToken ct) =>
            Task.FromResult(Videos.SingleOrDefault(video => video.Id == id));

        public Task AddAsync(Video entity, CancellationToken ct)
        {
            Operations.Add("database-add-new");
            if (FailAdd)
            {
                throw new InvalidOperationException("database add failed");
            }

            entity.Id = Videos.Count == 0 ? 1 : Videos.Max(video => video.Id) + 1;
            Videos.Add(entity);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Video entity, Action updateAction, CancellationToken ct)
        {
            updateAction();
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Video entity, CancellationToken ct)
        {
            Operations.Add(entity.BlobName == "old-promotion.mp4"
                ? "database-delete-old"
                : "database-delete-new");
            Videos.Remove(entity);
            return Task.CompletedTask;
        }

        public Task ReplacePromotionAsync(Video currentPromotion, Video replacement, CancellationToken ct)
        {
            Operations.Add("database-replace");
            if (FailReplace)
            {
                throw new InvalidOperationException("database replace failed");
            }

            Videos.Remove(currentPromotion);
            replacement.Id = currentPromotion.Id;
            Videos.Add(replacement);
            return Task.CompletedTask;
        }

        public Task<VideoFullProjection?> GetByIdAsNoTrackingAsync(long id, CancellationToken ct) =>
            Task.FromResult<VideoFullProjection?>(null);

        public Task<IEnumerable<VideoFullProjection>?> GetForPageAsync(
            int page,
            CancellationToken ct,
            int pageSize = 10,
            params object[] args) => Task.FromResult<IEnumerable<VideoFullProjection>?>([]);

        public Task<int> GetTotalCountAsync(CancellationToken ct, params object[] args) => Task.FromResult(0);

        public Task<VideoFullProjection?> GetPromotionVideoForHomePageAsync(CancellationToken ct) =>
            Task.FromResult<VideoFullProjection?>(null);

        public Task<Video?> GetPromotionVideoAsync(CancellationToken ct) =>
            Task.FromResult(Videos.SingleOrDefault(video => video.Type == VideoType.Promotion));

        public IReadOnlyList<string> CombinedOperations(RecordingMediaStorageService storage)
        {
            return storage.Timeline;
        }
    }

    private sealed class RecordingMediaStorageService : IMediaStorageService
    {
        public List<string> Operations { get; } = [];
        public List<string> Timeline { get; init; } = [];
        public bool FailUpload { get; init; }
        public bool FailOldBlobDelete { get; init; }

        public Task<StoredMedia> UploadAsync(
            IFormFile file,
            MediaKind mediaKind,
            string namePrefix,
            CancellationToken ct)
        {
            Operations.Add("upload");
            Timeline.Add("upload");
            if (FailUpload)
            {
                throw new InvalidOperationException("upload failed");
            }

            return Task.FromResult(new StoredMedia(
                "storonnimv-video",
                "video-new.mp4",
                "https://storage.test/storonnimv-video/video-new.mp4"));
        }

        public Task<string> CreateAsync(
            IFormFile file,
            MediaKind mediaKind,
            string namePrefix,
            Func<string, Task> persistAsync,
            CancellationToken ct) => throw new NotSupportedException();

        public Task<string> ReplaceAsync(
            IFormFile file,
            MediaKind mediaKind,
            string namePrefix,
            string? oldUrl,
            Func<string, Task> persistAsync,
            CancellationToken ct) => throw new NotSupportedException();

        public Task DeleteAsync(
            MediaKind mediaKind,
            string? oldUrl,
            Func<Task> persistAsync,
            CancellationToken ct) => throw new NotSupportedException();

        public async Task DeleteByBlobNameAsync(
            MediaKind mediaKind,
            string blobName,
            Func<Task> persistAsync,
            CancellationToken ct)
        {
            string operation = blobName == "old-promotion.mp4"
                ? "delete-old-promotion"
                : "delete-new-promotion";
            Operations.Add(operation);
            Timeline.Add(operation);
            await persistAsync();
            Operations.Add($"delete-blob:{blobName}");
            Timeline.Add($"delete-blob:{blobName}");

            if (FailOldBlobDelete && blobName == "old-promotion.mp4")
            {
                throw new MediaCleanupException("storonnimv-video", blobName, new InvalidOperationException("delete failed"));
            }
        }

        public Task DeleteUploadedAsync(StoredMedia media, CancellationToken ct)
        {
            Operations.Add("delete-new-blob");
            Timeline.Add("delete-new-blob");
            return Task.CompletedTask;
        }
    }
}
