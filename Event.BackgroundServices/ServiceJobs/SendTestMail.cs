using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Event.Application.Contracts;
using Event.Application.EmailService;
using Event.BackgroundService.ServiceBase;

namespace Event.BackgroundService.ServiceJobs
{
    public class SendTestMail : BackgroundServiceProcessor
    {
        public SendTestMail(IServiceScopeFactory serviceScopeFactory, IMailService mailService, IHostEnvironment env) : base(serviceScopeFactory, mailService, env)
        {
        }

        protected override string Schedule => "0 * * * *"; //runs every hour (https://crontab.cronhub.io/)

        protected override string JobName => "Get Currency Exchange Rates";

        public override async Task JobExecuteAsync(IServiceProvider serviceProvider)
        {
            try
            {
                //send mail
                //await _mailService.SendGridClient.SendMail("georgeebisike@gmail.com", "Hello Admin", "Clean Architecture project for Selenia has been installed");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
