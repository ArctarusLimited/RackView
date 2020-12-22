using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coordinator.Areas.Identity.Controllers
{
    [Route("Identity/[controller]")]
    [ApiController]
    [Authorize]
    public class OrganisationsController : ControllerBase
    {

    }
}
