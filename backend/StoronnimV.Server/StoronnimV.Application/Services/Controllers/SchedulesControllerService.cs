using AutoMapper;
using StoronnimV.Application.Contracts.Controllers;
using StoronnimV.Application.Contracts.Entities;
using StoronnimV.Application.DTO.Responses.SchedulePage;
using StoronnimV.Application.DTO.Responses.Shared;
using StoronnimV.Application.Models;
using StoronnimV.Domain.Projections.Schedule;

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
    public async Task<ScheduleResponse> GetItemByIdAsync(long id, CancellationToken ct)
    {
        ScheduleFullProjection schedule = await scheduleService.GetItemByIdAsync(id, ct);
        
        var scheduleDto = mapper.Map<ScheduleResponse>(schedule);
        
        return scheduleDto;
    }

    public async Task<PaginationResponse<ScheduleShortResponse>> GetForPageAsync(int page, int pageSize, CancellationToken ct, params object[] args)
    {
        PaginationResult<ScheduleShortProjection> paginationResult = await scheduleService.GetForPageAsync(page, pageSize, ct);

        var schedulesDto = mapper.Map<IEnumerable<ScheduleShortResponse>>(paginationResult.Items);

        var response = new PaginationResponse<ScheduleShortResponse>
        {
            CurrentPage = paginationResult.CurrentPage,
            TotalPages = paginationResult.TotalPages,
            TotalItems = paginationResult.TotalItems,
            Items = schedulesDto
        };
        
        return response;
    }
}