using System.ComponentModel.DataAnnotations;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing.Shared;

namespace StoronnimV.Application.DTO.Requests.Entities.Pages.Editing.Media;


public class EntityVideoEditRequest : BaseEditRequest
{
    [Range(1, long.MaxValue)]
    public long VideoId { get; set; }
}
