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
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ScheduleResponse>>> GetSchedules()
        {
            //TODO: illia - дописать
            return Ok();
        }
    }
}
