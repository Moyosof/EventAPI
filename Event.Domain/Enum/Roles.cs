using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event.Domain.Enum
{
    public enum Roles
    {
        Admin = 1,
        User = 2
    }

    public static class RoleType
    {
        public const string Admin = "Admin";
        public const string User = "User";
    }
}
