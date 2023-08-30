using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Event.Application.EmailService;
using Event.Application.EmailService.EmailClients;
using Event.Application.EmailService.Entities;

namespace Event.Infrastructure.EmailService
{
    public sealed class SendGridClient : ISendGridClient
    {
        private readonly SendGridKey sendgridApiKey;
        private readonly SendGridFrom sendGridClient;

        public SendGridClient(IOptions<SendGridKey> apikey, IOptions<SendGridFrom> sendGridClient)
        {
            sendgridApiKey = apikey.Value;
            this.sendGridClient = sendGridClient.Value;
        }
        public async Task<bool> SendMail(string reciever, string subject, string body, List<string> attachments = null)
        {
            var Receivers = new List<MailRecipientDTO>()
            {
                new MailRecipientDTO(){Address = reciever}
            };

            return await ExecuteSendGrid(Receivers, subject, body, attachments);
        }

        public async Task<bool> SendMail(MailRecipientDTO Receivers, string subject, string body, List<string> attachments = null)
        {
            return await ExecuteSendGrid(new List<MailRecipientDTO>() { Receivers }, subject, body, attachments);
        }

        public async Task<bool> SendMail(List<MailRecipientDTO> Receivers, string subject, string body, List<string> attachments = null)
        {
            return await ExecuteSendGrid(Receivers, subject, body, attachments);
        }

        #region SENDGRID API2
        private async Task<bool> ExecuteSendGrid(List<MailRecipientDTO> recipients, string subject, string body, List<string> attachments = null)
        {

            SendGridClientService sendGrid = new(sendgridApiKey.ApiKey, sendGridClient);

            string mode = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").Equals("Production") ? string.Empty : "[TEST MODE]";
            subject = $"{mode} {subject}";
            bool response = await sendGrid.SendMail(recipients, body, subject, attachments);
            return response;
        }
        #endregion
    }
}
