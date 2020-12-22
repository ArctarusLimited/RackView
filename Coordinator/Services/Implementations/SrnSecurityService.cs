using System;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Coordinator.Areas.Identity.Data;
using Coordinator.Models.Config;
using Microsoft.EntityFrameworkCore;

namespace Coordinator.Services.Implementations
{
    /// <summary>
    /// Enables access validation to SRN routes.
    /// </summary>
    public class SrnSecurityService : ISrnSecurityService
    {
        private readonly IdentityContext _context;
        public SrnSecurityService(IdentityContext context)
        {
            _context = context;
        }

        public async Task<bool> Match(string srn, ClaimsPrincipal principal)
        {
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) throw new Exception("Unable to find user ID claim in the JWT token.");

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == userId);
            if (user == null) throw new Exception("No such user could be found by the ID in the claim.");

            return await Match(srn, user);
        }

        public async Task<bool> Match(string srn, User user)
        {
            if (user.IsAdmin) return true;
            if (user.OrganisationId == null) return false;

            return await _context.SrnAuthAssignments.AnyAsync(
                a => 
                    a.OrganisationId == user.OrganisationId && 
                    Regex.IsMatch(srn, a.Route));
        }

        public async Task Register(string route, int? organisationId)
        {
            if (_context.SrnAuthAssignments.Any(a => a.Route == route && a.OrganisationId == organisationId))
                throw new Exception("The authorisation assignment already exists.");

            await _context.AddAsync(new SrnAuthAssignment {Route = route, OrganisationId = organisationId});
            await _context.SaveChangesAsync();
        }

        public async Task Unregister(string route, int? organisation)
        {
            var assignment =
                _context.SrnAuthAssignments.SingleOrDefaultAsync(a =>
                    a.Route == route && a.OrganisationId == organisation);
            if (assignment == null) return;

            _context.Remove(assignment);
            await _context.SaveChangesAsync();
        }
    }
}
