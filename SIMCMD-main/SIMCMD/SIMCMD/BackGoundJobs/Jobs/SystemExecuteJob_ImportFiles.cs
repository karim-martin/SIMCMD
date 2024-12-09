using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using SIMCMD.Data;
using SIMCMD.Models;

namespace SIMCMD.BackGroundJobs.Jobs
{
    [DisallowConcurrentExecution]
    public class SystemExecuteJob_ImportFiles : IJob 
    {
        private readonly ILogger<SystemExecuteJob_ImportFiles> _logger;
        private readonly IMyBackgroundJobs _backgroundJobs;
        private readonly ApplicationDbContext _dbContext;

        public SystemExecuteJob_ImportFiles(ILogger<SystemExecuteJob_ImportFiles> logger, IMyBackgroundJobs backgroundJobs, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _backgroundJobs = backgroundJobs;
            _dbContext = dbContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (_logger.BeginScope("{Job}", nameof(SystemExecuteJob_ImportFiles)))
            {
                _logger.LogDebug("Begining job execution");

                var watch = new System.Diagnostics.Stopwatch();
                bool isJobSuccessful = false;

                try
                {
                    watch.Start();

                    _backgroundJobs.ImportFiles();

                    isJobSuccessful = true;

                    _logger.LogDebug("Job execution completed successfully");
                }
                catch (OperationCanceledException)
                {
                    _logger.LogDebug("Operation canceled exception");
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "An unhandled exception was thrown.");
                }
                finally
                {
                    watch.Stop();
                    _logger.LogDebug("Job execution time: {JobExectionTime} ms", watch.ElapsedMilliseconds);

                    // Save to database here
                    var backgroundJob = new MyBackgroundJob
                    {
                        Title = "ImportFiles Job",
                        Description = "This job copies files from one local path to another.",
                        LastRun = DateTime.Now,
                        Successful = isJobSuccessful
                    };
                    _dbContext.MyBackgroundJob.Add(backgroundJob);
                    await _dbContext.SaveChangesAsync();
                }
            }
            await Task.CompletedTask;
        }
    }
}
