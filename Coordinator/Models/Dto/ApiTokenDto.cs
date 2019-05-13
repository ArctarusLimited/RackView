using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coordinator.Models.Dto
{
    public class ApiTokenDto
    {
        public int Id;
        public DateTime? Expiry;
        public string Notes;
    }
}
