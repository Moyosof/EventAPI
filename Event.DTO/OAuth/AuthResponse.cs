using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event.DTO.OAuth
{
    public record AuthResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; init; } = default;
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; init; } = default;

        [JsonProperty("username")]
        public string Username { get; init; } = default;


        [JsonProperty("phone_number")]
        public string PhoneNumber { get; init; } = default;

        [JsonProperty("email")]
        public string Email { get; init; } = default;

        [JsonProperty("user_id")]
        public string UserId { get; init; } = default;

        [JsonProperty("first_name")]
        public string FirstName { get; init; } = default;

        [JsonProperty("last_name")]
        public string LastName { get; init; } = default;

        [JsonProperty("middle_name")]
        public string MiddleName { get; init; } = default;

        [JsonProperty("fullname")]
        public string FullName { get; init; } = default;

        [JsonProperty("expiry_time")]
        public DateTime ExpiryTime { get; init; } = default;

        [JsonProperty("roles")]
        public string[] Roles { get; set; } = default;
    }
}
