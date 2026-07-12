using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing.Shared;

namespace StoronnimV.Application.DTO.Requests.Entities.Pages.Editing;

public class GroupPageEditRequest : BaseEditRequest
{
    public required string Description { get; init; }
}