using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coordinator.Services;

namespace Coordinator.Models.Config
{
    public interface ISrnProvider
    {
        /// <summary>
        /// Gets the namespaces, namespace or value at the specified SRN.
        /// </summary>
        Task<dynamic> GetAsync(Srn srn);

        /// <summary>
        /// Sets the value at the specified SRN.
        /// </summary>
        Task SetAsync(Srn srn, dynamic value);

        /// <summary>
        /// Deletes the namespace or value at the specified SRN.
        /// </summary>
        Task DeleteAsync(Srn srn);
    }
}
