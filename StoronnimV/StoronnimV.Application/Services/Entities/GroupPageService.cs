using StoronnimV.Contracts.Repositories;
using StoronnimV.Contracts.Services.Entities;

namespace StoronnimV.Application.Services.Entities;

public class GroupPageService(IGroupPageRepository groupPageRepository) : IGroupPageService
{
    private readonly IGroupPageRepository _groupPageRepository = groupPageRepository;
    public async Task<object> GetItemByIdAsync(long id)
    {
        var groupPage = await _groupPageRepository.GetByIdAsNoTrackingAsync(id);
        
        //TODO: Сделать ексепшн для ситуации, когда нет сущности по такому айди (GroupPageService)
        return groupPage ?? throw new Exception();
    }

    public async Task<IEnumerable<object>> GetAllAsync()
    {
        var groupPages = await _groupPageRepository.GetAllAsync();
        
        return groupPages ?? new List<object>();
    }
    
    public async Task<object> GetFirstGroupPageAsync()
    {
        var groupPage = await _groupPageRepository.GetFirstGroupPageAsync();
        
        //TODO: Сделать ексепшн для ситуации, когда нет сущности (GroupPageService)
        return groupPage ?? throw new Exception();
    }
}