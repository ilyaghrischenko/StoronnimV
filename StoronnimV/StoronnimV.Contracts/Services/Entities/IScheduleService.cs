using StoronnimV.Contracts.Services.Entities.Shared;

namespace StoronnimV.Contracts.Services.Entities;

public interface IScheduleService : IReceivableService
{
    Task UpdateStatusesAsync();
}