using Microsoft.Extensions.Configuration;
using Quartz;

namespace SIMCMD.Services.Scheduler
{
    public static class SystemSchedulerServiceExtensions
    {
        public static bool AddJobAndTrigger<T>(this IServiceCollectionQuartzConfigurator quartz, IConfiguration config)
            where T : IJob
        {
            // Use the name of the IJob as the appsettings.json key
            string jobName = typeof(T).Name;

            // Try and load the schedule from configuration
            var configKey = $"ServiceSetting:Quartz:{jobName}";
            var cronSchedule = config[configKey];

            // Some minor validation
            if (string.IsNullOrEmpty(cronSchedule))
            {
                return false;
            }

            // register the job as before
            var jobKey = new JobKey(jobName);
            quartz.AddJob<T>(opts => opts.WithIdentity(jobKey));

            quartz.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity(jobName + "-trigger")
                .WithCronSchedule(cronSchedule));

            return true;
        }
    }
}
