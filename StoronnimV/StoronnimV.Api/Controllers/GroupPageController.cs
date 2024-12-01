using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoronnimV.Contracts.Services.Controllers;
using StoronnimV.DTO.Responses.GroupPage;

namespace StoronnimV.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupPageController(IGroupPageControllerService groupPageControllerService) : ControllerBase
    {
        private readonly IGroupPageControllerService _groupPageControllerService = groupPageControllerService;
        
        [HttpGet]
        public async Task<ActionResult<GroupPageFullInfoResponse>> GetGroupPageInfoAsync()
        {
            var groupPage = await _groupPageControllerService.GetGroupPageInfoAsync();
            return Ok(groupPage);
        }
        
        [HttpGet("/member/{memberId}")]
        public async Task<ActionResult<MemberFullInfoResponse>> GetMemberInfoAsync(long memberId)
        {
            var member = await _groupPageControllerService.GetMemberInfoAsync(memberId);
            return Ok(member);
        }
    }
}
