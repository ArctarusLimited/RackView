using System;

namespace Coordinator.Models.Config
{
    /// <summary>
    /// Defines a SRN keyvalue backing provider.
    /// </summary>
    internal class SrnProviderAttribute : Attribute
    {
        internal string Name;
        internal bool Default;

        internal SrnProviderAttribute(string name, bool @default = false)
        {
            Name = name;
            Default = @default;
        }
    }
}
