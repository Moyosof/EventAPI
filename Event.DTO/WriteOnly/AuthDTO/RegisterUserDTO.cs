using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event.DTO.WriteOnly.AuthDTO
{
    public class RegisterUserDTO
    {
        [JsonProperty("FirstName")]
        [Required]
        public string FirstName { get; set; }
        [JsonProperty("LastName")]
        [Required]
        public string LastName { get; set; }
        [JsonProperty("EmailAddress")]
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
        [JsonProperty("PhoneNumber")]
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        [JsonProperty("Password")]
        [Required]
        [PasswordPropertyText(true)]
        public string Password { get; set; }
        [JsonProperty("ConfirmPassword")]
        [Required]
        [PasswordPropertyText(true)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
