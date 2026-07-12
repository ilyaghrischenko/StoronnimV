using StoronnimV.Application.Contracts.Controllers.Shared;
using StoronnimV.Application.DTO.Responses.MusicPage;

namespace StoronnimV.Application.Contracts.Controllers;

public interface IMusicControllerService
    : IGetByIdControllerService<MusicResponse>, IGetAllControllerService<MusicResponse>
{
    
}