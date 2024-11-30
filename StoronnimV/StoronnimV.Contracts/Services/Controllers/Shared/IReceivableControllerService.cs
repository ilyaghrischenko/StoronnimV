using StoronnimV.DTO.Responses.Shared;

namespace StoronnimV.Contracts.Services.Controllers.Shared;

public interface IReceivableControllerService<TDto> where TDto : BaseDto
{
    Task<TDto> GetItemByIdAsync(long id);
    Task<IEnumerable<TDto>> GetAllAsync();
}