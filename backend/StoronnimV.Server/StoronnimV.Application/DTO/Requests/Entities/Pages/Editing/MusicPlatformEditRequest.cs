using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing.Shared;

namespace StoronnimV.Application.DTO.Requests.Entities.Pages.Editing;

public class MusicPlatformEditRequest: BaseEditRequest
{
    public string PlatformUrl { get; set; } = string.Empty;
}
