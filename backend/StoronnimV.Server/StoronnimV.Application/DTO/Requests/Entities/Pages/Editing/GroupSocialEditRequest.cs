using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing.Shared;

namespace StoronnimV.Application.DTO.Requests.Entities.Pages.Editing;

public class GroupSocialEditRequest : BaseEditRequest
{
    public required string LinkUrl { get; init; }
}