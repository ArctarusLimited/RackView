using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coordinator.Models.Config;

namespace Coordinator.Services
{
    /// <summary>
    /// This service is part of the coordinator northbound API, and stores configuration, globally for the whole cluster or on a per-coordinator basis.
    /// </summary>
    /// <remarks>
    /// The configuration is stored in a JSON file per-namespace.
    /// An example of a valid global SRN would be urn:srn:v0:default:global:discovery.v4.enabled.
    /// An example of a valid scoped SRN would be urn:srn:v0:default:sup-a007299e-06b7-41ad-ab24-3a640383400c:discovery.v4.enabled.
    /// The SRN format is urn:srn:[version]:[provider]:[namespace]:[key].
    /// </remarks>
    public interface ISrnRepository
    {
        /// <summary>
        /// The system namespace. This stores attributes about the general
        /// operation of the server, for example, database credentials.
        /// </summary>
        SrnNamespace System { get; }

        /// <summary>
        /// The global namespace. This stores attributes applicable to
        /// all organisations and supervisors on the server.
        /// </summary>
        SrnNamespace Global { get; }

        /// <summary>
        /// Retrieves the value at the specified SRN.
        /// If no provider is specified, the repository will attempt
        /// to query all registered providers for a match.
        /// </summary>
        /// <param name="srn">The SRN to use.</param>
        /// <param name="provider">The provider to use.</param>
        /// <returns>The value of the SRN.</returns>
        Task<dynamic> GetAsync(Srn srn, string provider = "any");
        Task<dynamic> GetAsync(Srn srn, IList<string> providers);

        /// <summary>
        /// Sets the value at the specified SRN.
        /// </summary>
        /// <param name="srn">The SRN to use.</param>
        /// <param name="value">The value to set the SRN to.</param>
        /// <param name="provider">The provider to use.</param>
        Task SetAsync(Srn srn, dynamic value, string provider = "default");

        /// <summary>
        /// Deletes the value at the specified SRN.
        /// </summary>
        /// <param name="srn">The SRN to use.</param>
        /// <param name="provider">The provider to use.</param>
        Task DeleteAsync(Srn srn, string provider = "default");

        /// <summary>
        /// Retrieves a SRN provider by name.
        /// </summary>
        /// <param name="provider">The provider key.</param>
        ISrnProvider GetProvider(string provider);

        /// <summary>
        /// Retrieves all registered SRN providers.
        /// </summary>
        /// <returns>A <see cref="IReadOnlyDictionary{TKey,TValue}"/> of SRN providers.</returns>
        IReadOnlyDictionary<string, ISrnProvider> GetProviders();
    }
}
