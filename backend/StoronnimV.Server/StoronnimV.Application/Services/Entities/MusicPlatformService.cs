using StoronnimV.Application.Contracts.Entities;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Addition;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing.Media;
using StoronnimV.Application.Exceptions;
using StoronnimV.Domain.Contracts.AzureBlobStorage;
using StoronnimV.Domain.Contracts.Database;
using StoronnimV.Domain.Entities;
using StoronnimV.Domain.Projections;

namespace StoronnimV.Application.Services.Entities;

public class MusicPlatformService(
    IMusicPlatformRepository musicPlatformRepository,
    IBlobRepository blobRepository) : IMusicPlatformService
{
    public async Task<MusicPlatformProjection> GetItemByIdAsync(long id, CancellationToken ct)
    {
        MusicPlatformProjection musicPlatform = await musicPlatformRepository.GetByIdAsNoTrackingAsync(id, ct)
                                                ?? throw new EntityNotFoundException(
                                                    $"Music Platform with {nameof(id)}: {id} was not found");

        return musicPlatform;
    }

    public async Task<IEnumerable<MusicPlatformProjection>> GetAllAsync(CancellationToken ct)
    {
        var allMusicPlatforms = await musicPlatformRepository.GetAllAsNoTrackingAsync(ct);
        if (allMusicPlatforms is null || !allMusicPlatforms.Any())
        {
            return new List<MusicPlatformProjection>();
        }

        return allMusicPlatforms
            .ToList();
    }

    /// <summary>
    /// Music platform addition to database
    /// </summary>
    /// <param name="request">MusicPlatformAdditionRequest</param>
    /// <param name="ct">CancellationToken</param>
    public async Task AddMusicPlatformAsync(MusicPlatformAdditionRequest request, CancellationToken ct)
    {
        MusicPlatform musicPlatform = new()
        {
            BgImageUrl = "default",
            PlatformUrl = request.PlatformUrl
        };

        await musicPlatformRepository.AddAsync(musicPlatform, ct);

        string musicPlatformBlobName = $"music-platform-{musicPlatform.Id}";

        string extension = Path.GetExtension(request.BgImageUrl.FileName);
        string musicPlatformPhotoUrl = await blobRepository
            .AddFileAndGetUrlAsync("storonnimv-photo", $"{musicPlatformBlobName}{extension}",
                request.BgImageUrl.OpenReadStream(), ct);

        await musicPlatformRepository.UpdateAsync(musicPlatform, () => musicPlatform.BgImageUrl = musicPlatformPhotoUrl,
            ct);
    }

    /// <summary>
    /// Music platform deletion from database
    /// </summary>
    /// <param name="id">long</param>
    /// <param name="ct">CancellationToken</param>
    /// <exception cref="EntityNotFoundException"></exception>
    public async Task DeleteMusicPlatformAsync(long id, CancellationToken ct)
    {
        MusicPlatform? musicPlatform = await musicPlatformRepository.GetByIdAsync(id, ct);

        if (musicPlatform is null)
        {
            throw new EntityNotFoundException($"Music platform with {nameof(id)}: {id} was not found");
        }

        await musicPlatformRepository.DeleteAsync(musicPlatform, ct);

        await blobRepository.DeleteAllFilesByNameAsync("storonnimv-photo", $"music-platform-{id}", ct);
    }

    public async Task UpdateMusicPlatformAsync(MusicPlatformEditRequest request, CancellationToken ct)
    {
        MusicPlatform? musicPlatform = await musicPlatformRepository.GetByIdAsync(request.Id, ct);

        if (musicPlatform is null)
        {
            throw new EntityNotFoundException($"Music Platform with {nameof(request.Id)}: {request.Id} was not found");
        }

        await musicPlatformRepository.UpdateAsync(musicPlatform, () => musicPlatform.PlatformUrl = request.PlatformUrl,
            ct);
    }

    public async Task UpdateMusicPlatformPhotoAsync(PhotoEditRequest request, CancellationToken ct)
    {
        MusicPlatform? musicPlatform = await musicPlatformRepository.GetByIdAsync(request.Id, ct);

        if (musicPlatform is null)
        {
            throw new EntityNotFoundException($"Music Platform with {nameof(request.Id)}: {request.Id} was not found");
        }

        string musicPlatformBlobName = $"music-platform-{musicPlatform.Id}";
        await blobRepository.DeleteAllFilesByNameAsync("storonnimv-photo", musicPlatformBlobName, ct);

        string extension = Path.GetExtension(request.Photo.FileName);

        string musicPlatformPhotoUrl = await blobRepository.AddFileAndGetUrlAsync
            ("storonnimv-photo", $"{musicPlatformBlobName}{extension}", request.Photo.OpenReadStream(), ct);

        await musicPlatformRepository.UpdateAsync(musicPlatform, () => musicPlatform.BgImageUrl = musicPlatformPhotoUrl,
            ct);
    }
}