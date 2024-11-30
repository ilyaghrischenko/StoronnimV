using StoronnimV.DTO.Responses.GroupPage;

namespace StoronnimV.Contracts.Services.Controllers;

public interface IGroupPageControllerService
{
    public Task<GrouPageFullInfoResponse> GetGroupPageInfoAsync();
}