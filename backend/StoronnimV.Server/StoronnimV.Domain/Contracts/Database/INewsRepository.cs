using StoronnimV.Domain.Contracts.Database.Shared;
using StoronnimV.Domain.Entities;
using StoronnimV.Domain.Projections.News;

namespace StoronnimV.Domain.Contracts.Database;

public interface INewsRepository
    : IRepository<News>, IGetByIdRepository<NewsFullProjection>,
        IPaginationRepository<NewsPaginationProjection>
{
    Task<IEnumerable<NewsHomeProjection>?> GetMainNewsForHomePageAsync(int count, CancellationToken ct);
}