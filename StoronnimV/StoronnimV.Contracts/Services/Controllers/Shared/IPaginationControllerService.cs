using StoronnimV.DTO.Responses.Shared;

namespace StoronnimV.Contracts.Services.Controllers.Shared;

public interface IPaginationControllerService<TDto> where TDto : BaseDto
{
    Task<IEnumerable<TDto>> GetForPageAsync(int page);
}