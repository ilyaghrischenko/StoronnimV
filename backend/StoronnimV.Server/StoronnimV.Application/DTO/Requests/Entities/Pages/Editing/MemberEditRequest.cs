using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing.Shared;

namespace StoronnimV.Application.DTO.Requests.Entities.Pages.Editing;

public class MemberEditRequest : BaseEditRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}