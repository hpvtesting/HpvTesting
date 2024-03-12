using HPVTesting.Interfaces.Background;
using Hangfire;
using System;
using System.Linq.Expressions;

namespace HPVTesting.Services
{
    public class BackgroundService : IBackgroundService
    {
        public void EnqueueJob<TJobs>(Expression<Action<TJobs>> job) where TJobs : IBackgroundJobs
        {
            BackgroundJob.Enqueue(job);
        }

        public void ScheduleJob<TJobs>(Expression<Action<TJobs>> job, DateTimeOffset date) where TJobs : IBackgroundJobs
        {
            BackgroundJob.Schedule(job, date);
        }
    }
}