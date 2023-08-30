using Event.Application.EmailService.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Event.Application.EmailService.EmailClients
{
    /// <summary>
    /// This class sends mails using Gmail SMTP
    /// </summary>
    public class GmailClient
    {
        private readonly GmailOptions _gmailOption;

        public GmailClient(GmailOptions gmailOptions)
        {
            _gmailOption = gmailOptions;
        }
        public async Task SendMail(string subject, List<MailRecipientDTO> recipients, string body, List<string> attachments = null)
        {
            using (MailMessage mail = new(from: new MailAddress(_gmailOption.Email, _gmailOption.DisplayName), to: new MailAddress("notsent@afiari.com")))
            {
                mail.To.Clear(); //clear the mailing list to remove dummy email address

                //add recievers
                SetRecipients(recipients, mail);

                SetAttachments(attachments, mail);

                mail.Subject = subject;
                mail.Body = body;

                mail.IsBodyHtml = true;

                //create smtp client
                SmtpClient client = new()
                {
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    EnableSsl = true,
                    Host = "smtp.gmail.com",
                    Port = 587
                };

                //network credentials
                var credential = new NetworkCredential(_gmailOption.Email, _gmailOption.Password);
                client.UseDefaultCredentials = false;
                client.Credentials = credential;

                try
                {
                    await client.SendMailAsync(mail);
                }
                catch (Exception)
                {

                    return;
                }
            }
        }

        private static void SetAttachments(List<string> attachments, MailMessage mail)
        {
            if (attachments != null)
            {
                attachments.ForEach(item =>
                {
                    mail.Attachments.Add(new Attachment(item));
                });
            }
        }

        private static void SetRecipients(List<MailRecipientDTO> recievers, MailMessage mail)
        {

            recievers.ForEach(item =>
            {
                mail.To.Add(new MailAddress(address: item.Address, displayName: item.Name));
            });
        }
    }
}
