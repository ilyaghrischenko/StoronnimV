using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing.Shared;

namespace StoronnimV.Application.DTO.Requests.Entities.Pages.Editing.Media;


public class EntityVideoEditRequest : BaseEditRequest
{
    public long? VideoId { get; set; } = null;
}