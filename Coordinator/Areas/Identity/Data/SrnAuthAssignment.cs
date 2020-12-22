using System.ComponentModel.DataAnnotations;

namespace Coordinator.Areas.Identity.Data
{
    /// <summary>
    /// Assignment that binds an organisation to a specific route.
    /// </summary>
    public class SrnAuthAssignment
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The SRN route regex.
        /// </summary>
        public string Route { get; set; }

        /// <summary>
        /// Organisation who has control over this SRN route.
        /// If none is specified, it will be restricted to server administrators.
        /// </summary>
        public int? OrganisationId { get; set; }
    }
}
