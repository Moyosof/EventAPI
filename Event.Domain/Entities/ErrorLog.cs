using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event.Domain.Entities
{
    public class ErrorLog
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
        public string RequestUri { get; set; }
        public DateOnly DateLogged { get; set; }
        public TimeOnly TimeLogged { get; set; }
    }
}
