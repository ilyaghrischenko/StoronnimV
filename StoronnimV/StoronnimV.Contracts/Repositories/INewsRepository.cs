using StoronnimV.Contracts.Repositories.Shared;
using StoronnimV.Domain.Entities;

namespace StoronnimV.Contracts.Repositories;

public interface INewsRepository
    : IRepository<News>, IIncludable<News>, IReceivable<News>
{
    
}