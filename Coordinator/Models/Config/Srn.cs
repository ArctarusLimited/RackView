using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Coordinator.Services;

namespace Coordinator.Models.Config
{
    /// <summary>
    /// A SRN, or Supervisor Resource Name, is a custom Uniform Resource Name
    /// schema used to uniquely identify supervisor configuration parameters and actions.
    /// </summary>
    public class Srn
    {
        /// <summary>
        /// The version of the SRN. The only currently supported version is v0.
        /// </summary>
        internal short Version;

        /// <summary>
        /// The namespace of the SRN. Maximum length is 64 characters.
        /// </summary>
        internal string Namespace;

        /// <summary>
        /// The key of the SRN. Maximum length is 128 characters.
        /// </summary>
        internal string Key;

        internal Srn() { }

        internal Srn(Srn value)
        {
            Version = value.Version;
            Namespace = value.Namespace;
            Key = value.Key;
        }

        internal bool HasNamespace() => !string.IsNullOrWhiteSpace(Namespace);
        internal bool HasKey() => !string.IsNullOrWhiteSpace(Key);

        internal bool IsFullyQualified() => HasNamespace() && HasKey();

        internal void ThrowIfNotFullyQualified()
        {
            if (!IsFullyQualified()) throw new Exception("The SRN must be fully qualified.");
        }

        internal SrnNamespace ToNamespace(ISrnRepository repository)
        {
            return new SrnNamespace(Namespace, repository);
        }

        internal static Srn Parse(string srn)
        {
            var spl = srn.Split(':');
            if (spl.Length < 3)
                throw new InvalidOperationException("Invalid number of elements in the SRN.");

            // Validate protocol stuff
            if (spl[0] != "urn")
                throw new InvalidOperationException("URN protocol is invalid.");
            if (spl[1] != "srn")
                throw new InvalidOperationException("SRN descriptor is invalid.");
            if (string.IsNullOrWhiteSpace(spl[2]))
                throw new InvalidOperationException("SRN version is invalid.");

            var version = short.Parse(spl[2].Replace("v", ""));

            string nameSpace = null;
            string key = null;

            switch (version)
            {
                case 0:
                    // Parse v0 SRNs
                    if (spl.Length > 5)
                        throw new InvalidOperationException("Invalid number of elements in the SRN.");

                    // Namespace
                    if (spl.Length >= 4)
                    {
                        nameSpace = spl[3];

                        if (nameSpace.Length > 64)
                            throw new InvalidOperationException("SRN namespace is too long. The maximum number of characters in a namespace is 64.");
                        if (!Regex.IsMatch(nameSpace, @"^[a-zA-Z0-9_-]+$"))
                            throw new InvalidOperationException("SRN namespaces may only contain alphanumeric characters, underscores and dashes.");
                    }

                    // Key
                    if (spl.Length >= 5)
                    {
                        key = spl[4];

                        // Validate key
                        if (key.Length > 128)
                            throw new InvalidOperationException("SRN key is too long. The maximum number of characters in a key is 128.");
                        if (!Regex.IsMatch(key, @"^[a-zA-Z0-9.-]+$"))
                            throw new InvalidOperationException("SRN keys may only contain alphanumeric characters, dots and dashes");
                    }

                    return new Srn {Version = version, Key = key, Namespace = nameSpace};
                default:
                    throw new InvalidOperationException("Unsupported SRN version.");
            }
        }

        public override string ToString()
        {
            // Build the SRN
            var sb = new StringBuilder();

            sb.Append($"urn:srn:v{Version}");
            if (!string.IsNullOrWhiteSpace(Namespace)) sb.Append($":{Namespace}");
            if (!string.IsNullOrWhiteSpace(Key)) sb.Append($":{Key}");

            return sb.ToString();
        }

        public static implicit operator Srn(string str) => Parse(str);
    }
}
