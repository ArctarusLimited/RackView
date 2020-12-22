﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Coordinator.Areas.Identity.Data
{
    public class Organisation
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }


        // Foreign keys
        public string OwnerId { get; set; }
    }
}
