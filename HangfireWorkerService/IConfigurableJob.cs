using Hangfire.Server;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace HangfireWorkerService
{
    public interface IConfigurableJob
    {
        JobType JobType { get; }
        string Interval { get; }
        DateTimeOffset Delay { get; }
        string JobId { get; }
        string SectionName { get; }
        void Run(PerformContext context, IDictionary<string, string> configuration = null);
    }
}
