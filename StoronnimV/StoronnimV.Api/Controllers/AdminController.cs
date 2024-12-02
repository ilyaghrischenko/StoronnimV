using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace StoronnimV.Api.Controllers
{
    /// <summary>
    /// Контроллер для админа, он позволяет управлять данными, которые отображаются на страницых (Удалять, изменять)
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
    }
}
