using Event.Domain.ReadOnly;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event.Domain.Entities.Auth.AuthUser
{
    public class OneTimeCode
    {
        public OneTimeCode()
        {

        }
        public OneTimeCode(OneTimeCodeDTO oneTimeCodeDTO)
        {
            Id = Guid.NewGuid();
            Token = oneTimeCodeDTO.Token;
            Sender = oneTimeCodeDTO.Sender;
            IsUsed = false;
            ExpiringDate = DateTime.UtcNow.AddMinutes(15.0d);
        }

        [Key]
        public Guid Id { get; set; }
        public string Token { get; set; }
        public string Sender { get; set; }
        public bool IsUsed { get; set; }
        public DateTime ExpiringDate { get; set; }
    }
}
