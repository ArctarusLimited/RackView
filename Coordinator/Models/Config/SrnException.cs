using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coordinator.Models.Config
{
    public class SrnException : Exception
    {
        public SrnException(string message) : base(message) { }
        public SrnException(string message, Exception innerException) : base(message, innerException) { }
    }
}
