using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coordinator.Models.Config;
using Coordinator.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Coordinator.Helpers
{
    internal static class DbOptionsBuilderExtensions
    {
        /// <summary>
        /// Configures the specified DB options builder from the system SRN namespace.
        /// </summary>
        internal static async Task FromSrn(this DbContextOptionsBuilder builder, IServiceProvider provider)
        {
            // Get system scenario
            //?.LogDebug($"Configuring database.");

            var nameSpace = provider.GetService<ISrnRepository>().System;
            var env = provider.GetService<IHostingEnvironment>();

            var sql = await nameSpace.GetAsync("database");
            if (sql == null) return;
            foreach (var kv in sql)
            {
                var value = kv.Value;
                var scenario = (string) value["scenario"];

                if (!value["default"]) continue;

                // Check environment
                if (!env.IsScenario(scenario)) continue;

                switch (kv.Key)
                {
                    case "sqlserver":
                        //options.WithExtension
                        builder.UseSqlServer(ConnectionStringHelper.BuildSqlServer((IDictionary<string, dynamic>)value["connection"]));
                        break;
                    default:
                        //_logger.LogWarning($"Unsupported database provider {kv.Key}. Skipping.");
                        continue;
                }

                //_logger.LogInformation($"Registered {kv.Key} as the database provider for context {GetType()}.");
            }
        } 
    }
}
