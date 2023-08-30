using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event.DTO.OAuth
{
    public record JwtSettings
    {
        public string Site { get; set; }
        public string Secret { get; set; }
        public TimeSpan TokenLifeTime { get; set; }
        public string Audience { get; set; }
    }
}
