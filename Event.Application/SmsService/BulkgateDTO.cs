using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event.Application.SmsService
{
    public class BulkgateDTO
    {
        [JsonProperty("application_id")]
        public string application_id { get; set; }

        [JsonProperty("application_token")]
        public string application_token { get; set; }

        [DataType(DataType.PhoneNumber)]
        [JsonProperty("number")]
        public string number { get; set; }

        [JsonProperty("text")]
        public string text { get; set; }

        [JsonProperty("country")]
        public string country { get; set; } = "ng";
        [JsonProperty("unicode")]

        public bool unicode { get; set; } = true;
        [JsonProperty("schedule")]

        public string schedule { get; set; } = $"{DateTime.UtcNow}";
        [JsonProperty("sender_id_value")]

        public string sender_id_value { get; set; } = "BulkGate";
    }
}
