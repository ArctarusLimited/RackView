using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coordinator.Helpers;
using Coordinator.Models.Config;
using Coordinator.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Coordinator.Models.Database
{
    public class BaseContext : DbContext
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger _logger;

        public BaseContext(DbContextOptions options, IServiceProvider provider) : base(options)
        {
            _provider = provider;
            _logger = provider.GetService<ILoggerFactory>().CreateLogger("database");
        }

        protected override async void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            // Configure from the SRN namespace
            _logger.LogDebug($"Configuring database for context {GetType()}.");
            await optionsBuilder.FromSrn(_provider);
        }
    }
}
