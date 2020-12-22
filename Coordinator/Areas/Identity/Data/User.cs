using System;
using Microsoft.AspNetCore.Identity;

namespace Coordinator.Areas.Identity.Data
{
    public class User : IdentityUser
    {
        /// <summary>
        /// Whether the user is a server admin.
        /// </summary>
        public bool IsAdmin { get; set; }

        public int? OrganisationId { get; set; }
        public Organisation Organisation { get; set; }
    }
}
