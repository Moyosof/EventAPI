using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event.DTO.OAuth
{
    public record TokenRequestDTOw
    {
        public string RefreshToken { get; set; }
    }
}
