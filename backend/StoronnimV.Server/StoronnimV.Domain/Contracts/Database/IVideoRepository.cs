using StoronnimV.Domain.Contracts.Database.Shared;
using StoronnimV.Domain.Entities;
using StoronnimV.Domain.Projections.Video;

namespace StoronnimV.Domain.Contracts.Database;

public interface IVideoRepository 
    : IRepository<Video>, IGetByIdRepository<VideoFullProjection>,
        IPaginationRepository<VideoFullProjection>
{
    public Task<VideoFullProjection?> GetPromotionVideoForHomePageAsync(CancellationToken ct);
    public Task<Video?> GetPromotionVideoAsync(CancellationToken ct);
}