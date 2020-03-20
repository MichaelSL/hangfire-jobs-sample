using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.LiteDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HangfireWorkerService
{
    public class Worker : IHostedService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IJobRepository _jobRepository;
        private readonly IConfiguration _configuration;
        private BackgroundJobServer _backgroundJobServer;

        public Worker(ILogger<Worker> logger, IJobRepository jobRepository, IConfiguration configuration)
        {
            _logger = logger;
            _jobRepository = jobRepository ?? throw new ArgumentNullException(nameof(jobRepository));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger?.LogInformation("Starting...");
            GlobalConfiguration.Configuration.UseLiteDbStorage();
            _backgroundJobServer = new BackgroundJobServer();
            ScheduleJobs();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger?.LogInformation("Stopping...");
            _backgroundJobServer.Dispose();
            return Task.CompletedTask;
        }

        private void ScheduleJobs()
        {
            var jobsToSchedule = _jobRepository.GetJobs();
            foreach (var job in jobsToSchedule)
            {
                var section = new JobOptionsSection();

                if (job.SectionName != null)
                    _configuration.GetSection(job.SectionName).Bind(section);

                switch (job.JobType)
                {
                    case JobType.Recurring:
                        RecurringJob.AddOrUpdate(job.JobId ?? Guid.NewGuid().ToString(), () => job.Run(null, section.Values), job.Interval);
                        break;
                    case JobType.FireAndForget:
                        BackgroundJob.Enqueue(() => job.Run(null, section.Values));
                        break;
                    case JobType.Delayed:
                        BackgroundJob.Schedule(() => job.Run(null, section.Values), job.Delay);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(job.JobType), "Invalid JobType");
                }
            }
        }
    }
}
