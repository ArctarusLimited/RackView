using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace Coordinator.Services
{
    public class ApiSecurityService
    {
        internal readonly SymmetricSecurityKey SecurityKey;
        internal const string Issuer = "co.uk.arctarus";
        internal const string Audience = "co.uk.arctarus.rack.coordinator";
        public ApiSecurityService(ISrnRepository repository)
        {
            var result = repository.System.GetAsync("security.api.token", new[] { "default", "vault" }).GetAwaiter().GetResult();
            if (result == null) throw new Exception("Unable to initialise the API security service. Security key not found.");

            SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(result["key"]));
            
        }
    }
}
