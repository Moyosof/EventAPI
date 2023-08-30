using Event.Domain.ReadOnly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event.Application.EmailService
{
    public interface IFluentEmailClient
    {
        /// <summary>
        /// Sends email to the sender once the event is trigered
        /// </summary>
        /// <param name="source"></param>
        /// <param name="code"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SendOneTimeCodeEmail(Object source, OneTimeCodeDTO oneTimeCodeDTO, CancellationToken cancellationToken);

    }
}
