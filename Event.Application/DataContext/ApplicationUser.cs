using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace Event.Application.DataContext
{
    /// <summary>
    /// This class inherits from the IDentityUser Class
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateTime DateAddes { get; set; } = DateTime.UtcNow;
    }
}
