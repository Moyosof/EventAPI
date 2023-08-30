using Event.Application.EmailService;
using Event.Domain.ReadOnly;
using FluentEmail.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event.Infrastructure.EmailService
{
    public class FluentEmailClientService : IFluentEmailClient
    {
        private readonly IFluentEmail _fluentEmail;
        public FluentEmailClientService(IFluentEmail fluentEmail)
        {
            _fluentEmail = fluentEmail;
        }
        public async Task SendOneTimeCodeEmail(object source, OneTimeCodeDTO oneTimeCodeDTO, CancellationToken cancellationToken)
        {
            var emailTemplate = "<p>Your OTP code is <i>@Model.Token</i> .</p>";
            var newEmail = _fluentEmail
                .To(oneTimeCodeDTO.Sender)
                .Subject("[TestMode] One Time Code")
                .UsingTemplate<OneTimeCodeDTO>(emailTemplate, oneTimeCodeDTO);
            await newEmail.SendAsync(cancellationToken);
        }
    }
}
