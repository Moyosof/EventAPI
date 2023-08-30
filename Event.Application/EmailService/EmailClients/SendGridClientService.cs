﻿using Event.Application.EmailService.Entities;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Event.Application.EmailService.EmailClients
{
    /// <summary>
    /// This class sends mails using sendgrid
    /// </summary>
    public class SendGridClientService
    {
        private readonly SendGridClient _sendGridClient;
        private readonly SendGridFrom _sender;

        public SendGridClientService(string secret_key, SendGridFrom sender)
        {
            if (string.IsNullOrEmpty(secret_key))
            {
                throw new ArgumentException($"'{nameof(secret_key)}' cannot be null or empty.", nameof(secret_key));
            }

            _sendGridClient = new SendGridClient(secret_key);
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        public async Task<bool> SendMail(List<MailRecipientDTO> recipients, string body, string subject, List<string> attachments = null)
        {
            SendGridMessage message = CreateMessageInstance(subject, body);//, from);

            var Sender = From();

            var Recipients = To(recipients);

            message.From = Sender;

            message.AddTos(Recipients);

            SetAttachments(attachments, message);

            try
            {
                Response Response = await _sendGridClient.SendEmailAsync(message);
                bool status_code = Response.IsSuccessStatusCode;
                return status_code;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static SendGridMessage CreateMessageInstance(string subject, string body)
        {
            return new SendGridMessage()
            {
                Subject = subject,
                PlainTextContent = body,
                HtmlContent = body,
            };
        }

        private EmailAddress From()
        {
            EmailAddress From = new(_sender.Email, _sender.DisplayName);
            return From;
        }

        private static List<EmailAddress> To(List<MailRecipientDTO> recievers)
        {
            var Recievers = new List<EmailAddress>();
            recievers.ForEach(item =>
            {
                Recievers.Add(new EmailAddress() { Email = item.Address, Name = item.Name });
            });

            return Recievers;
        }

        private static void SetAttachments(List<string> attachments, SendGridMessage SendGridMessageInstance)
        {
            var Attachments = new List<Attachment>();
            if (attachments != null)
            {
                attachments.ForEach(item =>
                {
                    Attachments.Add(new Attachment() { Content = item });
                });
                SendGridMessageInstance.AddAttachments(Attachments);
            }
        }

        private static List<Attachment> SetAttachments(List<string> attachments)
        {
            var Attachments = new List<Attachment>();
            if (attachments != null)
            {
                attachments.ForEach(item =>
                {
                    Attachments.Add(new Attachment() { Content = item });
                });
                return Attachments;
            }

            return new();
        }
    }
}
