using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using VaultSharp;
using VaultSharp.Core;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.SecretsEngines.KeyValue.V2;

namespace Coordinator.Models.Config.Vault
{
    [SrnProvider("vault")]
    public class VaultSrnProvider : ISrnProvider
    {
        private IVaultClient _client;
        private readonly IKeyValueSecretsEngineV2 _engine;
        private readonly string _mountPoint;

        public VaultSrnProvider(IConfiguration options)
        {
            _mountPoint = options["MountPoint"];

            // Instantiate the client
            var authMethod = new TokenAuthMethodInfo(options["Token"]);
            var settings = new VaultClientSettings(options["Url"], authMethod);

            _client = new VaultClient(settings);
            _engine = _client.V1.Secrets.KeyValue.V2;
        }

        public async Task<dynamic> GetAsync(Srn srn)
        {
            return await ExecuteVaultContext(async () =>
            {
                // List all namespaces
                if (!srn.HasNamespace())
                    return (await _engine.ReadSecretPathsAsync("/", _mountPoint))?.Data.Keys;

                // List all keys in namespace (non-recursive, though!)
                if (!srn.HasKey())
                    return (await _engine.ReadSecretPathsAsync($"/{srn.Namespace}", _mountPoint))?.Data.Keys;

                return (await _engine.ReadSecretAsync(GetKeyPath(srn), null, _mountPoint))?.Data.Data;
            });
        }

        public async Task SetAsync(Srn srn, dynamic value)
        {
            srn.ThrowIfNotFullyQualified();
            await ExecuteVaultContext(async () => await _engine.WriteSecretAsync(GetKeyPath(srn), value, null, _mountPoint));
        }

        public async Task DeleteAsync(Srn srn)
        {
            // TODO: need to find out how to get version
            if (!srn.HasNamespace()) throw new SrnException("Performing a mount point reset is not yet supported.");
            await ExecuteVaultContext(async () =>
                {
                    await _engine.DestroySecretAsync(GetKeyPath(srn), new List<int> {0}, _mountPoint);
                    return Task.CompletedTask;
                });
        }

        private static async Task<dynamic> ExecuteVaultContext(Func<Task<dynamic>> action)
        {
            try
            {
                return await action.Invoke();
            }
            catch (VaultApiException e)
            {
                if (e.HttpStatusCode == HttpStatusCode.NotFound) return null;

                // Bad stuff
                throw new SrnException("Vault encountered an internal error.", e);
            }
            catch (Exception e)
            {
                throw new SrnException("Failed to connect to Vault.", e);
            }
        }

        private static string GetKeyPath(Srn srn) => $"{srn.Namespace}/{srn.Key.Replace('.', '/')}";
    }
}
