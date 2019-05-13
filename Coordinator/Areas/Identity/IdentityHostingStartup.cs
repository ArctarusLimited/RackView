using System;
using Coordinator.Areas.Identity.Data;
using Coordinator.Models;
using Coordinator.Models.Database;
using Coordinator.Services;
using Coordinator.Services.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

[assembly: HostingStartup(typeof(Coordinator.Areas.Identity.IdentityHostingStartup))]
namespace Coordinator.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                services.Configure<CookiePolicyOptions>(options =>
                {
                    options.CheckConsentNeeded = ctx => true;
                    options.MinimumSameSitePolicy = SameSiteMode.Strict;
                });

                services.AddDbContext<IdentityContext>();
                services.AddSingleton<ApiSecurityService>();

                // Add the default identity provider
                services.AddDefaultIdentity<User>()
                    .AddDefaultUI(UIFramework.Bootstrap4)
                    .AddEntityFrameworkStores<IdentityContext>();
                services.AddAuthentication(options =>
                        {
                            options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                            options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                        })
                    .AddJwtBearer();

                // Add configuration providers
                services.AddSingleton<IPostConfigureOptions<JwtBearerOptions>, JwtBearerConfigureOptions>();

                services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
                services.AddSingleton<IAuthenticationSchemeProvider, CoordinatorSchemeProvider>();

                services.ConfigureApplicationCookie(options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(10);

                    options.SlidingExpiration = true;
                });
            });
        }
    }
}