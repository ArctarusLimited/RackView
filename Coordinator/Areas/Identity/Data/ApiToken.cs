﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Coordinator.Areas.Identity.Data
{
    public class ApiToken
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The autogenerated API token.
        /// </summary>
        [MaxLength(32)]
        public string Token { get; set; }

        /// <summary>
        /// The expiry of the token. After this date, it will no longer be valid.
        /// </summary>
        public DateTime? Expiry { get; set; }

        /// <summary>
        /// Additional notes to store with the token.
        /// </summary>
        [MaxLength(64)]
        public string Notes { get; set; }

        /// <summary>
        /// The user that created this token.
        /// It is able to delete the token.
        /// </summary>
        public string UserId { get; set; }
    }
}
