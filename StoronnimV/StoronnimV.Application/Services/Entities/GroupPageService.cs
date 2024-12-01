using StoronnimV.Application.Exceptions;
using StoronnimV.Contracts.Repositories;
using StoronnimV.Contracts.Services.Entities;

namespace StoronnimV.Application.Services.Entities;

public class GroupPageService(IGroupPageRepository groupPageRepository) : IGroupPageService
{
    private readonly IGroupPageRepository _groupPageRepository = groupPageRepository;
    
    public async Task<object> GetItemByIdAsync(long id)
    {
        var groupPage = await _groupPageRepository.GetByIdAsNoTrackingAsync(id)
            ?? throw new EntityNotFoundException($"GroupPage with id: {id} was not found");
        
        return groupPage;
    }

    public async Task<IEnumerable<object>> GetAllAsync()
    {
        var groupPages = await _groupPageRepository.GetAllAsync();
        
        return groupPages ?? new List<object>();
    }
    
    public async Task<object> GetFirstGroupPageAsync()
    {
        var groupPage = await _groupPageRepository.GetFirstGroupPageAsync()
            ?? throw new EntityNotFoundException($"GroupPage was not found");
        
        return groupPage;
    }
}