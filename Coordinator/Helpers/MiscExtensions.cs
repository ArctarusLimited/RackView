using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace Coordinator.Helpers
{
    internal static class MiscExtensions
    {
        internal static bool IsScenario(this IHostingEnvironment env, string scenario)
        {
            switch (scenario)
            {
                case "dev":
                    return env.IsDevelopment();
                case "staging":
                    return env.IsStaging();
                case "live":
                    return env.IsProduction();
                default:
                    return false;
            }
        }
    }
}
