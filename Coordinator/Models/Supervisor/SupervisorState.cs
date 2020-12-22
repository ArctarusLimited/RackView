using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coordinator.Models.Supervisor
{
    /// <summary>
    /// Represents the operational state of a Supervisor.
    /// </summary>
    public enum SupervisorState
    {
        /// <summary>
        /// An enrollment request has been queued, and the Coordinator is waiting for
        /// the Supervisor to go through enrollment to proceed.
        /// </summary>
        Enrolling,

        /// <summary>
        /// An enrollment request was queued but the Supervisor failed to
        /// complete the request within the set timeframe.
        /// </summary>
        Failed,

        /// <summary>
        /// The enrollment request was sent but the Coordinator is waiting for
        /// an organisation administrator to approve the request.
        /// </summary>
        Approve,

        /// <summary>
        /// The supervisor is offline. It will switch to this state
        /// after heartbeat has failed for an extended period of time.
        /// </summary>
        Offline,

        /// <summary>
        /// The supervisor is online and functioning normally.
        /// </summary>
        Online,
    }
}
