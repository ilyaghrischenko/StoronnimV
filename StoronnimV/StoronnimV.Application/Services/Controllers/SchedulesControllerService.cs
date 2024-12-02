using AutoMapper;
using StoronnimV.Contracts.Services.Controllers;
using StoronnimV.Contracts.Services.Entities;
using StoronnimV.DTO.Responses.SchedulePage;

namespace StoronnimV.Application.Services.Controllers;

/// <summary>
/// Сервис для маппинга данных с бд и возвращения контроллеру
/// </summary>
/// <param name="scheduleService"></param>
/// <param name="mapper"></param>
public class SchedulesControllerService(
    IScheduleService scheduleService,
    IMapper mapper) : ISchedulesControllerService
{
    private readonly IScheduleService _scheduleService = scheduleService;
    private readonly IMapper _mapper = mapper;
    
    public async Task<ScheduleResponse> GetItemByIdAsync(long id)
    {
        var schedule = await _scheduleService.GetItemByIdAsync(id);
        
        var scheduleDto = _mapper.Map<ScheduleResponse>(schedule);
        return scheduleDto;
    }

    public async Task<IEnumerable<ScheduleShortResponse>> GetAllAsync()
    {
        var schedules = await _scheduleService.GetAllAsync();
        
        var schedulesDto = _mapper.Map<IEnumerable<ScheduleShortResponse>>(schedules);
        return schedulesDto;
    }
}