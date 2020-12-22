using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coordinator.Models.Supervisor
{
    /// <summary>
    /// Represents the mode of a supervisor.
    /// </summary>
    public enum SupervisorMode
    {
        /// <summary>
        /// While in Initialisation mode, the Supervisor can be enrolled
        /// into any RackView Coordinator. It is not secure.
        /// </summary>
        Initialisation,

        /// <summary>
        /// When in Development mode, the Supervisor is enrolled in a Coordinator,
        /// however the development web interface and other dev stuff is enabled.
        /// For internal development use only.
        /// </summary>
        Development,

        /// <summary>
        /// Production mode is normal operation, a live Supervisor enrolled against a live Coordinator.
        /// </summary>
        Production,
    }
}
