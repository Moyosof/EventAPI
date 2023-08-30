using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event.DTO.WriteOnly.AuthDTO
{
    public class ForgetPasswordDTO
    {
        [Required]
        [JsonProperty("Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [JsonProperty("ResetPassword")]
        [Url]
        public string ResetPasswordPageLink { get; set; }
    }
}
