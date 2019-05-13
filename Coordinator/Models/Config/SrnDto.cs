using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coordinator.Helpers;
using Newtonsoft.Json;

namespace Coordinator.Models.Config
{
    public class SrnDto
    {
        [JsonConverter(typeof(DynamicDictConverter<Dictionary<string, dynamic>>))]
        public Dictionary<string, dynamic> Data { get; set; }

        public string Provider { get; set; } = "default";
        public List<string> Providers { get; set; }
    }
}
