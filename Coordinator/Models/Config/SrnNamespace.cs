using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coordinator.Services;

namespace Coordinator.Models.Config
{
    public class SrnNamespace
    {
        private readonly string _nameSpace;
        private readonly ISrnRepository _repository;
        public SrnNamespace(string nameSpace, ISrnRepository repository)
        {
            _nameSpace = nameSpace;
            _repository = repository;
        }

        public async Task<dynamic> GetAsync(string key, IList<string> providers, bool defaultDir = false)
        {
            var result = await GetAsyncInternal(key, providers);
            if (defaultDir && result is IDictionary<string, dynamic> dict) return dict.SingleOrDefault();

            return result;
        }

        private async Task<dynamic> GetAsyncInternal(string key, IList<string> providers)
        {
            var srn = new Srn { Namespace = _nameSpace, Key = key };

            // System namespace forces the JSON provider
            if (_nameSpace == "system" && key != null && !key.StartsWith("security"))
                return await _repository.GetProvider("json").GetAsync(srn);

            // If we're not looking for all just return a specific result.
            if (providers.Count == 1 && providers.SingleOrDefault() != "any")
                return await _repository.GetProvider(providers.SingleOrDefault()).GetAsync(srn);

            if (!srn.HasNamespace() || !srn.HasKey())
                throw new SrnException("Bulk queries support fully qualified SRNs only.");

            // This is the code that merges results together from all providers into one result.
            // The code will always return the first instance of a result. Duplicates are ignored.
            foreach (var (k, v) in _repository.GetProviders())
            {
                // allow only whitelisted providers
                if (!providers.Contains(k)) continue;

                var result = await v.GetAsync(srn);
                if (result != null) return result;
            }

            // No match found in any provider :(
            return null;
        }

        public async Task<dynamic> GetAsync(string key, string provider = "any") => await GetAsync(key, new List<string> {provider});

        public Task SetAsync(string key, dynamic value, string provider = "default")
        {
            if (_nameSpace == "system") provider = "json";
            return _repository.GetProvider(provider).SetAsync(new Srn { Namespace = _nameSpace, Key = key }, value);
        }

        public Task DeleteAsync(string key, string provider = "default")
        {
            if (_nameSpace == "system") provider = "json";
            return _repository.GetProvider(provider).DeleteAsync(new Srn { Namespace = _nameSpace, Key = key });
        }
    }
}
