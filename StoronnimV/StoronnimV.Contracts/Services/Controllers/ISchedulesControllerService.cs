using StoronnimV.Contracts.Services.Controllers.Shared;
using StoronnimV.DTO.Responses.SchedulePage;

namespace StoronnimV.Contracts.Services.Controllers;

public interface ISchedulesControllerService : IReceivableControllerService<ScheduleResponse>
{
    Task<IEnumerable<ScheduleShortResponse>> GetForPageAsync(int page);
}