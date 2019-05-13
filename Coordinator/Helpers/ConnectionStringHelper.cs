using System.Collections.Generic;
using System.Text;

namespace Coordinator.Helpers
{
    public static class ConnectionStringHelper
    {
        internal static string BuildSqlServer(IDictionary<string, dynamic> connection)
        {
            if (!connection.ContainsKey("server") || !connection.ContainsKey("database")) return null;
            var sb = new StringBuilder();

            sb.Append($"Server={connection["server"]};Database={connection["database"]}");
            if (connection.ContainsKey("trusted")) sb.Append($";Trusted_Connection={connection["trusted"]}");
            if (connection.ContainsKey("retrycount")) sb.Append($";ConnectRetryCount={connection["retrycount"]}");

            return sb.ToString();
        }
    }
}
