using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Coordinator.Models.Config;
using Microsoft.Extensions.DependencyInjection;

namespace Coordinator.Services
{
    /// <summary>
    /// Base implementation of <see cref="ISrnRepository"/>.
    /// </summary>
    public class SrnRepository : ISrnRepository
    {
        private readonly IReadOnlyDictionary<string, ISrnProvider> _providers;

        private readonly Srn _systemSrn = new Srn {Namespace = "system"};
        private readonly Srn _globalSrn = new Srn {Namespace = "global"};

        public SrnNamespace System { get; private set; }
        public SrnNamespace Global { get; private set; }

        public SrnRepository(IServiceProvider provider)
        {
            // Grab all our services and stick them in Providers with the proper keys
            var dict = new Dictionary<string, ISrnProvider>();
            foreach (var srnProvider in provider.GetServices<ISrnProvider>()) RegisterProvider(srnProvider, dict);

            // Convert to read only
            _providers = new ReadOnlyDictionary<string, ISrnProvider>(dict);

            // Register internal namespaces so that we can use them in startup classes
            RegisterInternalNamespaces();
        }

        public async Task<dynamic> GetAsync(Srn srn, string provider = "any")
            => await srn.ToNamespace(this).GetAsync(srn.Key, provider);

        public async Task<dynamic> GetAsync(Srn srn, IList<string> providers)
            => await srn.ToNamespace(this).GetAsync(srn.Key, providers);

        public async Task SetAsync(Srn srn, dynamic value, string provider = "default")
            => await srn.ToNamespace(this).SetAsync(srn.Key, value, provider);

        public async Task DeleteAsync(Srn srn, string provider = "default")
            => await srn.ToNamespace(this).DeleteAsync(srn.Key, provider);

        /// <summary>
        /// Registers a SRN provider with this service.
        /// </summary>
        /// <param name="provider">The provider instance.</param>
        /// <param name="registry">The dictionary to register the provider in.</param>
        private static void RegisterProvider(ISrnProvider provider, IDictionary<string, ISrnProvider> registry)
        {
            var attributes = provider.GetType().GetCustomAttributes(typeof(SrnProviderAttribute), false);
            if (attributes.Length != 1) throw new InvalidOperationException("SRN provider attribute was not found on the provider class.");

            var attribute = (SrnProviderAttribute)attributes.First();
            if (string.IsNullOrWhiteSpace(attribute.Name)) throw new InvalidOperationException("SRN provider attribute is invalid.");

            registry[attribute.Name] = provider;
            if (attribute.Default) registry["default"] = provider;
        }

        private void RegisterInternalNamespaces()
        {
            System = _systemSrn.ToNamespace(this);
            Global = _globalSrn.ToNamespace(this);
        }

        /// <summary>
        /// Retrieves a SRN provider by name.
        /// </summary>
        /// <param name="provider">The provider key.</param>
        public ISrnProvider GetProvider(string provider)
        {
            if (!_providers.ContainsKey(provider))
                throw new InvalidOperationException("The specified provider is not registered.");
            return _providers[provider];
        }

        public IReadOnlyDictionary<string, ISrnProvider> GetProviders() => _providers;

        //public async Task<SrnNamespace> GetNamespaceAsync(Srn srn) => await GetProvider(srn.Provider).GetAsync(srn);
    }
}
