using AutoMapper;
using StoronnimV.Application.Contracts.Controllers;
using StoronnimV.Application.Contracts.Entities;
using StoronnimV.Application.DTO.Responses.MusicPage;
using StoronnimV.Domain.Projections;

namespace StoronnimV.Application.Services.Controllers;

public class MusicControllerService(
    IMusicPlatformService musicPlatformService,
    IMapper mapper) : IMusicControllerService
{
    public async Task<MusicResponse> GetItemByIdAsync(long id, CancellationToken ct)
    {
        MusicPlatformProjection musicPlatformItem = await musicPlatformService.GetItemByIdAsync(id, ct);

        var musicPlatformDto = mapper.Map<MusicResponse>(musicPlatformItem);
        
        return musicPlatformDto;
    }

    public async Task<IEnumerable<MusicResponse>> GetAllAsync(CancellationToken ct)
    {
        var musicPlatforms = await musicPlatformService.GetAllAsync(ct);

        var musicPlatformsDto = mapper.Map<IEnumerable<MusicResponse>>(musicPlatforms);
        
        return musicPlatformsDto;
    }
}