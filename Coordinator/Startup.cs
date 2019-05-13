using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coordinator.Helpers;
using Coordinator.Models.Config;
using Coordinator.Models.Config.Json;
using Coordinator.Models.Config.Vault;
using Coordinator.Models.Database;
using Coordinator.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Coordinator
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        private IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // ** Registers SRN providers. The order they are registered here
            // defines the order they will be queried in when doing a bulk query.

            // JSON is always registered, kept as a fallback.
            services.AddSingleton<ISrnProvider, JsonSrnProvider>();

            var providers = Configuration.GetSection("SrnProviders");
            if (providers != null)
            {
                if (providers.GetSection("vault") is IConfigurationSection section) services.AddSingleton<ISrnProvider, VaultSrnProvider>(p => new VaultSrnProvider(section));
            }

            // Register the repo
            services.AddSingleton<ISrnRepository, SrnRepository>();

            // Register EF database
            services.AddDbContext<CoordinatorContext>();

            // TEST
            //var sps = services.BuildServiceProvider().GetService<CoordinatorDbContext>().SaveChanges();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
