using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event.Application.EmailService
{
    /// <summary>
    /// This service is used in sending emails
    /// </summary>
    public interface IMailService
    {
        ISendGridClient SendGridClient { get; }

        IGmailClient GmailClient { get; }
    }
}
