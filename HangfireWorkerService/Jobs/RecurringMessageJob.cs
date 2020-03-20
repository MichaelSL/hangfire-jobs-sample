using System;
using System.Collections.Generic;
using Hangfire;
using Hangfire.Server;
using Microsoft.Extensions.Configuration;

namespace HangfireWorkerService.Jobs
{
    public class RecurringMessageJob : IConfigurableJob
    {
        public JobType JobType => JobType.Recurring;

        public string Interval => Cron.Minutely();

        public DateTimeOffset Delay => throw new NotImplementedException();

        public string JobId => null;

        public string SectionName => null;

        public void Run(PerformContext context, IDictionary<string, string> configuration)
        {
            BackgroundJob.ContinueJobWith(context.BackgroundJob.Id, () => Console.WriteLine($"{DateTime.UtcNow.ToLongTimeString()}: Hardcoded continuation message!"));
            Console.WriteLine($"{DateTime.UtcNow.ToLongTimeString()}: {context.BackgroundJob.Id}: Hardcoded recurring message!");
        }
    }
}
