using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event.Domain.Entities.Auth
{
    public class RefreshToken
    {
        [Key]
        public Guid Id { get; init; }
        /// <summary>
        /// // Linked to the AspNet Identity User Id
        /// </summary>
        public string UserId { get; set; }
        public string Token { get; set; }
        /// <summary>
        /// // Map the token with jwtId
        /// </summary>
        public string JwtId { get; set; }
        /// <summary>
        /// // if its used we dont want generate a new Jwt token with the same refresh token
        /// </summary>
        public bool IsUsed { get; set; }
        /// <summary>
        /// // if it has been revoke for security reasons
        /// </summary>
        public bool IsRevoked { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime JwtExpiryDate { get; set; }
        /// <summary>
        /// // Refresh token is long lived it could last for months.
        /// </summary>
        public DateTime ExpiryDate { get; set; }
    }
}
