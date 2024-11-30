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
    public async Task<ScheduleResponse> GetItemByIdAsync(long id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<ScheduleShortResponse>> GetAllAsync()
    {
        throw new NotImplementedException();
    }
}