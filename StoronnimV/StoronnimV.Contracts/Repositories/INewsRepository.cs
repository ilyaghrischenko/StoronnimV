using StoronnimV.Contracts.Repositories.Shared;
using StoronnimV.Domain.DbModels;

namespace StoronnimV.Contracts.Repositories;

public interface INewsRepository
    : IRepository<News>, IIncludable<News>, IReceivable<News>
{
    
}