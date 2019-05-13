using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Coordinator.Areas.Identity.Data;
using Coordinator.Areas.Identity.Pages.Account.Manage;
using Coordinator.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Coordinator.Controllers
{
    [Route("api/v0/[controller]")]
    [ValidateAntiForgeryToken]
    [ApiController]
    [Authorize]
    public class TokenController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IdentityContext _context;

        public TokenController(UserManager<User> userManager, IdentityContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAll()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound("The specified user could not be found.");

            return new JsonResult(from t in _context.ApiTokens where t.UserId == user.Id select new ApiTokenDto { Id = t.Id, Expiry = t.Expiry, Notes = t.Notes });
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound("The specified user could not be found.");

            using (var rng = new RNGCryptoServiceProvider())
            {
                var data = new byte[16];
                rng.GetBytes(data);

                var token = new ApiToken
                {
                    Token = Convert.ToBase64String(data),
                    UserId = user.Id
                };

                _context.ApiTokens.Add(token);
                await _context.SaveChangesAsync();

                return new JsonResult(token);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put(ApiTokenDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound("The specified user could not be found.");

            if (dto == null) return BadRequest();
            var token = await _context.ApiTokens.SingleOrDefaultAsync(t => t.UserId == user.Id && t.Id == dto.Id);
            if (token == null) return NotFound();

            token.Expiry = dto.Expiry;
            token.Notes = dto.Notes;
            await _context.SaveChangesAsync();

            return new OkResult();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(ApiTokenDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound("The specified user could not be found.");

            if (dto == null) return BadRequest();
            var token = await _context.ApiTokens.SingleOrDefaultAsync(t => t.UserId == user.Id && t.Id == dto.Id);
            if (token == null) return NotFound();

            _context.Remove(token);
            await _context.SaveChangesAsync();

            return new OkResult();
        }
    }
}
