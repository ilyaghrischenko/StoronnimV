using StoronnimV.DTO.Responses.SchedulePage;

namespace StoronnimV.Contracts.Services.Controllers;

public interface ISchedulesControllerService
{
    Task<ScheduleResponse> GetItemByIdAsync(long id);
    Task<IEnumerable<ScheduleShortResponse>> GetAllAsync();
}