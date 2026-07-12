using StoronnimV.Application.DTO.Responses.Shared;

namespace StoronnimV.Application.Contracts.Controllers.Shared;

public interface IPaginationControllerService<TDto> where TDto : BaseResponseDto
{
    Task<PaginationResponse<TDto>> GetForPageAsync(int page, int pageSize, CancellationToken ct, params object[] args);
}