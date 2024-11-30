using AutoMapper;
using StoronnimV.Contracts.Services.Controllers;
using StoronnimV.Contracts.Services.Entities;

namespace StoronnimV.Application.Services.Controllers;

public class SchedulesControllerService(
    IScheduleService scheduleService,
    IMapper mapper) : ISchedulesControllerService
{
    private readonly IScheduleService _scheduleService = scheduleService;
    private readonly IMapper _mapper = mapper;
    
    
}