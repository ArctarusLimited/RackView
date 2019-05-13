using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Coordinator.Services.Options
{
    public class JwtBearerConfigureOptions : IPostConfigureOptions<JwtBearerOptions>
    {
        private readonly ApiSecurityService _service;
        public JwtBearerConfigureOptions(ApiSecurityService service)
        {
            _service = service;
        }

        public void PostConfigure(string name, JwtBearerOptions options)
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = ApiSecurityService.Issuer,
                ValidAudience = ApiSecurityService.Audience,
                IssuerSigningKey = _service.SecurityKey
            };
        }
    }
}
