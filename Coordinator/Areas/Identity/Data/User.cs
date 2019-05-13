using System;
using Microsoft.AspNetCore.Identity;

namespace Coordinator.Areas.Identity.Data
{
    public class User : IdentityUser
    {
        public Guid? OrganisationId { get; set; }
        public Organisation Organisation { get; set; }
    }
}
