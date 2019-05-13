using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Coordinator.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Coordinator.Areas.Identity.Pages.Account.Manage
{
    public class ApiTokensModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IdentityContext _context;

        public IEnumerable<ApiTokenDto> ApiTokens;

        [TempData]
        public string StatusMessage { get; set; }

        public ApiTokensModel(UserManager<User> userManager, IdentityContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public class ApiTokenDto
        {
            public int Id;
            public DateTime? Expiry;
            public string Notes;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound("The specified user could not be found.");

            ApiTokens = from t in _context.ApiTokens select new ApiTokenDto {Id = t.Id, Expiry = t.Expiry, Notes = t.Notes};

            return Page();
        }

        public async Task<IActionResult> OnPostGenerateAsync()
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

        //public async Task<IActionResult> OnDeleteAsync()
        //{
        //
        //}
    }
}