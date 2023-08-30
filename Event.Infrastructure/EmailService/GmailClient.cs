using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Event.Application.EmailService;
using Event.Application.EmailService.Entities;

namespace Event.Infrastructure.EmailService
{
    public sealed class GmailClient : IGmailClient
    {
        private readonly GmailOptions gmailOptions;

        public GmailClient(GmailOptions gmailOptions)
        {
            this.gmailOptions = gmailOptions;
        }

        public Task SendMail(string reciever, string subject, string body, List<string> attachments = null)
        {
            var Receivers = new List<MailRecipientDTO>()
            {
                new MailRecipientDTO(){Address = reciever}
            };

            return ExecuteGmailSMTP(Receivers, subject, body, attachments);
        }

        public Task SendMail(MailRecipientDTO Receivers, string subject, string body, List<string> attachments = null)
        {
            return ExecuteGmailSMTP(new List<MailRecipientDTO>() { Receivers }, subject, body, attachments);
        }

        public Task SendMail(List<MailRecipientDTO> Receivers, string subject, string body, List<string> attachments = null)
        {
            return ExecuteGmailSMTP(Receivers, subject, body, attachments);
        }



        #region Gmail SMTP 2
        private async Task ExecuteGmailSMTP(List<MailRecipientDTO> recipients, string subject, string body, List<string> attachments = null)
        {
            Application.EmailService.EmailClients.GmailClient gmailSMTP = new(gmailOptions);

            string mode = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").Equals("Production") ? string.Empty : "[TEST MODE]";
            subject = $"{mode} {subject}";
            await gmailSMTP.SendMail(subject, recipients, body, attachments);
        }
        #endregion
    }
}
