using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Coordinator.Areas.Identity.Data;
using Coordinator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Coordinator.Controllers
{
    /// <summary>
    /// Controller that handles obtaining JWT tokens
    /// from API tokens a user has.
    /// </summary>
    [Route("api/v0/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class JwtController : Controller
    {
        private readonly IdentityContext _context;
        private readonly SigningCredentials _credentials;

        public JwtController(IdentityContext context, ApiSecurityService security)
        {
            _context = context;
            _credentials = new SigningCredentials(security.SecurityKey, SecurityAlgorithms.HmacSha256Signature);
        }

        [HttpPost("obtain")]
        public async Task<IActionResult> Authenticate([FromBody] string apiToken)
        {
            var token = await _context.ApiTokens.SingleOrDefaultAsync(t => t.Token == apiToken);
            if (token == null || token.Expiry >= DateTime.Now) return BadRequest(new {message = "The API token is invalid."});

            // Find the user
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == token.UserId);
            if (user == null) return BadRequest(new {message = "Internal error: user is invalid."});

            // Grant the JWT token.
            var handler = new JwtSecurityTokenHandler();
            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = ApiSecurityService.Issuer,
                Audience = ApiSecurityService.Audience,

                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = _credentials,

                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Id),
                    new Claim(ClaimTypes.Role, "Owner"), // TODO: TEST ONLY!!!!
                    //new Claim("Organisation", user.OrganisationId.ToString())
                })
            };

            var jwt = handler.CreateToken(descriptor);
            return Json(handler.WriteToken(jwt));
        }
    }
}
