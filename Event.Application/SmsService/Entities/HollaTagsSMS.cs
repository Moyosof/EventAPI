using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event.Application.SmsService.Entities
{
    public class HollaTagsSMS
    {
        public string Url { get; set; }
        public string Cookie { get; set; }
        public string Username { get; set; }
        public string Pass { get; set; }
        public string From { get; set; }
        public string CallbackUrl { get; set; }
    }
}
