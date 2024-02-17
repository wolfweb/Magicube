using Magicube.Core;
using Microsoft.Extensions.DependencyInjection;
using NCrontab;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.ScheduleBackgroundTask {
    public abstract class SchedulerBackgroundTask : ScopedProcessor {
        protected virtual string   Schedule        { get; set; } = "*/10 * * * *";

        private readonly CrontabSchedule _schedule;
        private DateTime _nextRun;

        public SchedulerBackgroundTask(IServiceScopeFactory serviceScopeFactory) : base( serviceScopeFactory) {
            _schedule = CrontabSchedule.Parse(Schedule);
            _nextRun  = _schedule.GetNextOccurrence(DateTime.Now);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            do {
                var now = DateTime.Now;
                _schedule.GetNextOccurrence(now);

                if (now > _nextRun) {
                    await ProcessBackgroundTask(stoppingToken);
                    _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
                }

                await Task.Delay(5000, stoppingToken);
            }
            while (!stoppingToken.IsCancellationRequested);
        }
    }
}
