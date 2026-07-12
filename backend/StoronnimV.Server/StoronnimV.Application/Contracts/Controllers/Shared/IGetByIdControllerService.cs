using StoronnimV.Application.DTO.Responses.Shared;

namespace StoronnimV.Application.Contracts.Controllers.Shared;

public interface IGetByIdControllerService<TDto> where TDto : BaseResponseDto
{
    Task<TDto> GetItemByIdAsync(long id, CancellationToken ct);
}