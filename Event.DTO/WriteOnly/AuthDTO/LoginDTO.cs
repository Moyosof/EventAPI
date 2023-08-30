using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event.DTO.WriteOnly.AuthDTO
{
    public class LoginDTO
    {
        [Required]
        [JsonProperty("Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [JsonProperty("Password")]
        [PasswordPropertyText(true)]
        public string Password { get; set; }
    }
}
