using System;
using System.Linq.Expressions;

namespace HPVTesting.Interfaces.Background
{
    public interface IBackgroundService
    {
        void EnqueueJob<TJobs>(Expression<Action<TJobs>> job) where TJobs : IBackgroundJobs;

        void ScheduleJob<TJobs>(Expression<Action<TJobs>> job, DateTimeOffset date) where TJobs : IBackgroundJobs;
    }
}