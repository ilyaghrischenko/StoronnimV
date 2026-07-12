using StoronnimV.Application.Contracts.Controllers.Shared;
using StoronnimV.Application.DTO.Responses.Video;

namespace StoronnimV.Application.Contracts.Controllers;

public interface IVideoControllerService
    : IGetByIdControllerService<VideoPageResponse>, IPaginationControllerService<VideoPageResponse>
{
    
}