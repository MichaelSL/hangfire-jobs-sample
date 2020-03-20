using System.Collections.Generic;

namespace HangfireWorkerService
{
    public interface IJobRepository
    {
        IEnumerable<IConfigurableJob> GetJobs();
    }
}