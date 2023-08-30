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
    public class ChangePasswordDTO
    {
        [JsonProperty("CurrentPassword")]
        [Required]
        [PasswordPropertyText(true)]
        public string CurrentPassword { get; set; }

        [JsonProperty("NewPassword")]
        [Required]
        [PasswordPropertyText(true)]
        public string NewPassword { get; set; }

        [JsonProperty("ConfirmPassword")]
        [Required]
        [PasswordPropertyText(true)]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}
