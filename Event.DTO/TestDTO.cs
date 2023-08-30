using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event.DTO
{
    public class TestDTO
    {
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("pass")]

        public string pass { get; set; }
    }
}
