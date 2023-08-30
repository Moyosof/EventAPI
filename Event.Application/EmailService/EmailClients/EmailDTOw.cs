using Event.Application.EmailService.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event.Application.EmailService.EmailClients
{
    public class EmailDTOwBase
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<string> Attachments { get; set; }
    }

    public class EmailDTOwObject : EmailDTOwBase
    {
        public string RecieverAddress { get; set; }
        public string RecieverName { get; set; }
    }

    public class EmailDTOList : EmailDTOwBase
    {
        public List<MailRecipientDTO> Recipients { get; set; }
    }
}
