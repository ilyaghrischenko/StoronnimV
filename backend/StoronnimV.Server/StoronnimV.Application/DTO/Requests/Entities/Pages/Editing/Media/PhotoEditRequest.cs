using Microsoft.AspNetCore.Http;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing.Shared;

namespace StoronnimV.Application.DTO.Requests.Entities.Pages.Editing.Media;

public class PhotoEditRequest : BaseEditRequest
{
    public IFormFile Photo { get; set; } = null!;
}