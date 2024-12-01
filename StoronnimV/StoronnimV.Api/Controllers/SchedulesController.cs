using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoronnimV.Contracts.Services.Controllers;
using StoronnimV.Domain.Entities;
using StoronnimV.DTO.Responses.SchedulePage;

namespace StoronnimV.Api.Controllers
{
    [Route("api/schedules")]
    [ApiController]
    public class SchedulesController(ISchedulesControllerService schedulesControllerService) : ControllerBase
    {
        private readonly ISchedulesControllerService _schedulesControllerService = schedulesControllerService;

        [HttpGet("{id}")]
        public async Task<ActionResult<ScheduleResponse>> GetSchedule([FromRoute] long id)
        {
            var schedule = await _schedulesControllerService.GetItemByIdAsync(id);
            return Ok(schedule);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ScheduleShortResponse>>> GetSchedules()
        {
            var schedules = await _schedulesControllerService.GetAllAsync();
            return Ok(schedules);
        }
    }
}