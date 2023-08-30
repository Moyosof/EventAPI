using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event.DTO
{
    public record Result<T>
    {
        [JsonProperty("succeeded")]
        public bool Succeeded { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public List<T> Data { get; set; }

        [JsonProperty("meta_data")]
        public MetaData MetaData { get; set; }
    }
}
