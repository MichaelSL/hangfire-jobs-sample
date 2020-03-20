using System;
using System.Collections.Generic;
using Hangfire;
using Hangfire.Server;

namespace HangfireWorkerService.Jobs
{
    public class RecurringConfigurableMessageJob : IConfigurableJob
    {
        public JobType JobType => JobType.Recurring;

        public string Interval => Cron.Minutely();

        public DateTimeOffset Delay => throw new NotImplementedException();

        public string JobId => nameof(RecurringConfigurableMessageJob);

        public string SectionName => nameof(RecurringConfigurableMessageJob);

        public void Run(PerformContext context, IDictionary<string, string> configuration)
        {
            Console.WriteLine($"{DateTime.UtcNow.ToLongTimeString()}: {context.BackgroundJob.Id}: {configuration["Message"]}");
        }
    }
}
