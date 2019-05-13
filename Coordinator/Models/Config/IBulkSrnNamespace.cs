using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coordinator.Models.Config
{
    /// <summary>
    /// Defines a SRN namespace that can be bulk serialised
    /// to an output, for example, a JSON dictionary.
    /// </summary>
    public interface IBulkSrnNamespace
    {
        dynamic AsSerializable();
    }
}
