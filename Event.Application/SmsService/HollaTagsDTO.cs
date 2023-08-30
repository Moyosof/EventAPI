using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event.Application.SmsService
{
    public class HollaTagsDTO
    {
        [JsonProperty("user")]
        public string user { get; set; }

        [JsonProperty("pass")]
        public string pass { get; set; }

        [JsonProperty("from")]
        public string from { get; set; }

        [JsonProperty("to")]
        public string to { get; set; }

        [JsonProperty("msg")]
        public string msg { get; set; }

        [JsonProperty("type")]
        public int type { get; set; } = 0;

        [JsonProperty("callback_url")]
        public string callback_url { get; set; }

        [JsonProperty("enable_msg_id")]
        public bool enable_msg_id { get; set; } = true;
    }
}
