using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing.Shared;

namespace StoronnimV.Application.DTO.Requests.Entities.Pages.Editing;

public class VideoEditRequest: BaseEditRequest
{
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}