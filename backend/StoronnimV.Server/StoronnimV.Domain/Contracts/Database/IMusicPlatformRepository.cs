using StoronnimV.Domain.Contracts.Database.Shared;
using StoronnimV.Domain.Entities;
using StoronnimV.Domain.Projections;

namespace StoronnimV.Domain.Contracts.Database;

public interface IMusicPlatformRepository
    : IRepository<MusicPlatform>, IGetByIdRepository<MusicPlatformProjection>, IGetAllRepository<MusicPlatformProjection>
{
    
}