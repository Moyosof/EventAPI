using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event.Domain.ReadOnly
{
    public class OneTimeCodeDTO
    {
        public required string Token { get; set; }
        public required string Sender { get; set; }
    }
}
