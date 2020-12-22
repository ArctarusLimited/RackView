using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Coordinator.Areas.Identity.Data;
using Coordinator.Models.Config;

namespace Coordinator.Services
{
    public interface ISrnSecurityService
    {
        Task<bool> Match(string srn, ClaimsPrincipal principal);

        Task Register(string route, int? organisation);
        Task Unregister(string route, int? organisation);
    }
}
