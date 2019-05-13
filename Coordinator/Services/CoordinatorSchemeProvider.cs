using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Coordinator.Services
{
    /// <summary>
    /// This class provides a way to override authentication schemes on a per-route basis.
    /// </summary>
    public class CoordinatorSchemeProvider : AuthenticationSchemeProvider
    {
        private readonly IHttpContextAccessor _accessor;
        public CoordinatorSchemeProvider(IOptions<AuthenticationOptions> options, IHttpContextAccessor accessor) : base(options)
        {
            _accessor = accessor;
        }

        private async Task<AuthenticationScheme> GetDefinedSchemeAsync()
        {
            var request = _accessor.HttpContext?.Request;
            if (request == null) throw new Exception("The HTTP request was not defined.");

            // Override API requests with JWT tokens.
            if (request.Path.StartsWithSegments("/api"))
                return await GetSchemeAsync(JwtBearerDefaults.AuthenticationScheme);

            return null;
        }

        public override async Task<AuthenticationScheme> GetDefaultAuthenticateSchemeAsync() =>
            await GetDefinedSchemeAsync() ?? await base.GetDefaultAuthenticateSchemeAsync();

        public override async Task<AuthenticationScheme> GetDefaultChallengeSchemeAsync() =>
            await GetDefinedSchemeAsync() ?? await base.GetDefaultChallengeSchemeAsync();

        public override async Task<AuthenticationScheme> GetDefaultForbidSchemeAsync() =>
            await GetDefinedSchemeAsync() ?? await base.GetDefaultForbidSchemeAsync();

        public override async Task<AuthenticationScheme> GetDefaultSignInSchemeAsync() =>
            await GetDefinedSchemeAsync() ?? await base.GetDefaultSignInSchemeAsync();

        public override async Task<AuthenticationScheme> GetDefaultSignOutSchemeAsync() =>
            await GetDefinedSchemeAsync() ?? await base.GetDefaultSignOutSchemeAsync();
    }
}
