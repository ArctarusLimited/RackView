using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coordinator.Models.Config.Json
{
    /// <summary>
    /// Dictionary with some overrides to correctly parse in and expand SRN keypaths.
    /// </summary>
    public class JsonSrnDictionary : Dictionary<string, dynamic>
    {
        internal new dynamic this[string key]
        {
            get
            {
                // Dot paths use nested dictionaries
                var idx = key.IndexOf('.');
                if (idx != -1)
                {
                    var firstKey = key.Substring(0, idx);

                    // Find our nested dictionary
                    if (!ContainsKey(firstKey) || base[firstKey].GetType() != typeof(JsonSrnDictionary)) return null;
                    var str = key.Substring(idx + 1);
                    return base[firstKey][str];
                }

                return ContainsKey(key) ? base[key] : null;
            }
            set
            {
                // Dot paths use nested dictionaries
                var idx = key.IndexOf('.');
                if (idx != -1)
                {
                    var firstKey = key.Substring(0, idx);

                    // Create our nested dictionary
                    if (!ContainsKey(firstKey) || base[firstKey].GetType() != typeof(JsonSrnDictionary)) base[firstKey] = new JsonSrnDictionary();
                    base[firstKey][key.Substring(idx + 1)] = value;

                    return;
                }

                if (value == null)
                {
                    // Setting the value to null should remove it.
                    Remove(key);
                    return;
                }

                base[key] = value;
            }
        }
    }
}
