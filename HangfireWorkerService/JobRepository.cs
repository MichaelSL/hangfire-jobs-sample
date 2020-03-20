using HangfireWorkerService.Jobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace HangfireWorkerService
{
    public class JobRepository : IJobRepository
    {
        public IEnumerable<IConfigurableJob> GetJobs()
        {
            return new IConfigurableJob[]
            {
                new RecurringMessageJob(),
                new RecurringConfigurableMessageJob()
            };
        }
    }
}
