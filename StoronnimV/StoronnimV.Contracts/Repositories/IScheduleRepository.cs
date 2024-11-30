using StoronnimV.Contracts.Repositories.Shared;
using StoronnimV.Domain.Entities;

namespace StoronnimV.Contracts.Repositories;

public interface IScheduleRepository
    : IRepository<Schedule>, IReceivable<Schedule>
{
    
}