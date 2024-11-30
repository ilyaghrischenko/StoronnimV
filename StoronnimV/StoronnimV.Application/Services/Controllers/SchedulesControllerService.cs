using AutoMapper;
using StoronnimV.Contracts.Services.Controllers;
using StoronnimV.Contracts.Services.Entities;
using StoronnimV.DTO.Responses.SchedulePage;

namespace StoronnimV.Application.Services.Controllers;

public class SchedulesControllerService(
    IScheduleService scheduleService,
    IMapper mapper) : ISchedulesControllerService
{
    private readonly IScheduleService _scheduleService = scheduleService;
    private readonly IMapper _mapper = mapper;
    
    //TODO: illia - дописать 
    public Task<ScheduleResponse> GetItemByIdAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ScheduleResponse>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ScheduleShortResponse>> GetForPageAsync(int page)
    {
        throw new NotImplementedException();
    }
}