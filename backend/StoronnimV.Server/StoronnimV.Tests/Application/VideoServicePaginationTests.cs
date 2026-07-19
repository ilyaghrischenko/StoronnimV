using Microsoft.AspNetCore.Http;
using StoronnimV.Application.Contracts.Utils;
using StoronnimV.Application.Enums;
using StoronnimV.Application.Exceptions;
using StoronnimV.Application.Models;
using StoronnimV.Application.Services.Entities;
using StoronnimV.Domain.Contracts.Database;
using StoronnimV.Domain.Entities;
using StoronnimV.Domain.Projections.Video;

namespace StoronnimV.Tests.Application;

public sealed class VideoServicePaginationTests
{
    [Fact]
    public async Task GetForPageAsync_OutOfRangePage_PreservesCategoryTotals()
    {
        StubVideoRepository repository = new()
        {
            TotalCount = 3,
            PageItems = []
        };
        VideoService service = new(repository, new UnusedMediaStorageService());

        PaginationResult<VideoFullProjection> result = await service.GetForPageAsync(
            999,
            2,
            CancellationToken.None,
            "Performance");

        Assert.Equal(999, result.CurrentPage);
        Assert.Equal(2, result.TotalPages);
        Assert.Equal(3, result.TotalItems);
        Assert.Empty(result.Items);
    }

    [Theory]
    [InlineData(0, 2)]
    [InlineData(1, 0)]
    public async Task GetForPageAsync_NonPositivePagination_ThrowsPaginationException(
        int page,
        int pageSize)
    {
        VideoService service = new(new StubVideoRepository(), new UnusedMediaStorageService());

        await Assert.ThrowsAsync<PaginationException>(() => service.GetForPageAsync(
            page,
            pageSize,
            CancellationToken.None,
            "Performance"));
    }

    [Fact]
    public async Task GetForPageAsync_UnknownCategory_ThrowsPaginationExceptionBeforeRepositoryAccess()
    {
        StubVideoRepository repository = new();
        VideoService service = new(repository, new UnusedMediaStorageService());

        await Assert.ThrowsAsync<PaginationException>(() => service.GetForPageAsync(
            1,
            2,
            CancellationToken.None,
            "Unknown"));

        Assert.Equal(0, repository.TotalCountCalls);
    }

    private sealed class StubVideoRepository : IVideoRepository
    {
        public int TotalCount { get; init; }
        public IEnumerable<VideoFullProjection> PageItems { get; init; } = [];
        public int TotalCountCalls { get; private set; }

        public Task<VideoFullProjection?> GetByIdAsNoTrackingAsync(long id, CancellationToken ct) =>
            Task.FromResult<VideoFullProjection?>(null);

        public Task<Video?> GetByIdAsync(long id, CancellationToken ct) =>
            Task.FromResult<Video?>(null);

        public Task AddAsync(Video entity, CancellationToken ct) => Task.CompletedTask;

        public Task UpdateAsync(Video entity, Action updateAction, CancellationToken ct) => Task.CompletedTask;

        public Task DeleteAsync(Video entity, CancellationToken ct) => Task.CompletedTask;

        public Task<IEnumerable<VideoFullProjection>?> GetForPageAsync(
            int page,
            CancellationToken ct,
            int pageSize = 10,
            params object[] args) => Task.FromResult<IEnumerable<VideoFullProjection>?>(PageItems);

        public Task<int> GetTotalCountAsync(CancellationToken ct, params object[] args)
        {
            TotalCountCalls++;
            return Task.FromResult(TotalCount);
        }

        public Task<VideoFullProjection?> GetPromotionVideoForHomePageAsync(CancellationToken ct) =>
            Task.FromResult<VideoFullProjection?>(null);

        public Task<Video?> GetPromotionVideoAsync(CancellationToken ct) => Task.FromResult<Video?>(null);

        public Task ReplacePromotionAsync(Video currentPromotion, Video replacement, CancellationToken ct) =>
            Task.CompletedTask;
    }

    private sealed class UnusedMediaStorageService : IMediaStorageService
    {
        public Task<StoredMedia> UploadAsync(
            IFormFile file,
            MediaKind mediaKind,
            string namePrefix,
            CancellationToken ct) => throw new NotSupportedException();

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

        public Task DeleteByBlobNameAsync(
            MediaKind mediaKind,
            string blobName,
            Func<Task> persistAsync,
            CancellationToken ct) => throw new NotSupportedException();

        public Task DeleteUploadedAsync(StoredMedia media, CancellationToken ct) =>
            throw new NotSupportedException();
    }
}
