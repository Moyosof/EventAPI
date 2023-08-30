using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NCrontab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Event.Application.Contracts;
using Event.Application.EmailService;

namespace Event.BackgroundService.ServiceBase
{
    public abstract class BackgroundServiceProcessor : Microsoft.Extensions.Hosting.BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private CrontabSchedule _schedule;
        private DateTime _nextDate;
        protected readonly IMailService _mailService;
        protected readonly IHostEnvironment _env;

        private DateTime UTCTime { get { return DateTime.Now.ToUniversalTime().AddHours(1); } } //GMT +1

        public BackgroundServiceProcessor(IServiceScopeFactory serviceScopeFactory, IMailService mailService, IHostEnvironment env)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _schedule = CrontabSchedule.Parse(Schedule);
            _nextDate = _schedule.GetNextOccurrence(UTCTime);
            _mailService = mailService;
            _env = env;
        }

        protected abstract string Schedule { get; }
        protected abstract string JobName { get; }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                //get the current date from the UTCTime
                var currentDate = UTCTime;

                if (currentDate > _nextDate)
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        await JobExecuteAsync(scope.ServiceProvider);
                    }

                    //get the next scheduled occurance afater execution
                    _nextDate = _schedule.GetNextOccurrence(UTCTime);
                }

                await Task.Delay(15000, stoppingToken);
            } while (!stoppingToken.IsCancellationRequested);
        }

        public abstract Task JobExecuteAsync(IServiceProvider serviceProvider);
    }
}
