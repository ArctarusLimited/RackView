using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coordinator.Helpers;
using Coordinator.Models.Config;
using Coordinator.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Coordinator.Models.Database
{
    public class CoordinatorContext : BaseContext
    {
        public CoordinatorContext(DbContextOptions options, IServiceProvider serviceProvider) : base(options, serviceProvider) { }
    }
}
