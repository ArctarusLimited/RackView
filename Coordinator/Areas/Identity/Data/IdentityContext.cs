using System;
using Coordinator.Helpers;
using Coordinator.Models.Config;
using Coordinator.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Coordinator.Areas.Identity.Data
{
    public class IdentityContext : IdentityDbContext<User>
    {
        private readonly IServiceProvider _provider;

        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<ApiToken> ApiTokens { get; set; }

        // ReSharper disable once SuggestBaseTypeForParameter
        public IdentityContext(DbContextOptions<IdentityContext> options, IServiceProvider provider) : base(options)
        {
            _provider = provider;
        }

        protected override async void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            // Configure from the SRN namespace
            await optionsBuilder.FromSrn(_provider);
        }
    }
}
