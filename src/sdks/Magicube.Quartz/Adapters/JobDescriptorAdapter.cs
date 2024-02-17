using Magicube.Quartz.Models;
using Quartz;
using System;

namespace Magicube.Quartz.Jobs {
    public class JobDescriptorAdapter {
        private readonly JobDescriptor _jobDescription;

        public JobDescriptorAdapter(JobDescriptor model) {
            _jobDescription = model;
        }

        public IJobDetail RetrieveJobDetail<TJob>() where TJob : IJob {
            return RetrieveJobDetail(typeof(TJob));
        }

        public IJobDetail RetrieveJobDetail(Type type, Action<JobBuilder> config = null) {
            if (!typeof(IJob).IsAssignableFrom(type)) throw new QuartzException("type must implemented IJob");

            var jobBuilder = JobBuilder
                .Create(type)
                .SetJobData(new JobDataMap { { "Desc", _jobDescription.Description } })
                .WithIdentity(_jobDescription.JobName, _jobDescription.JobGroup);

            config?.Invoke(jobBuilder);

            return jobBuilder.Build();
        }

        public ITrigger RetrieveJobTrigger() {
            var builder = TriggerBuilder.Create()
                .WithIdentity(_jobDescription.JobName, _jobDescription.JobGroup);

            if (_jobDescription.JobData != null) {
                builder.UsingJobData(new JobDataMap(_jobDescription.JobData.ToDictionary()));
            }

            if (_jobDescription.Schedule.TimeOf != null) {
                int hour   = _jobDescription.Schedule.TimeOf.Hour,
                    minute = _jobDescription.Schedule.TimeOf.Minute,
                    second = _jobDescription.Schedule.TimeOf.Second;

                DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, minute, second, DateTimeKind.Local);
                if (_jobDescription.Schedule.RepeatCount > 0) {
                    builder.WithDailyTimeIntervalSchedule(x => {
                        x.WithIntervalInHours(24)
                        .OnEveryDay()
                        .WithRepeatCount(_jobDescription.Schedule.RepeatCount)
                        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(hour, minute));                        
                    }).StartNow();
                } else {
                    builder.WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(hour, minute)).StartNow();
                }
            } else {
                var timespan = BuildInterval(_jobDescription.Schedule.IntervalType, _jobDescription.Schedule.IntervalStep);
                builder.WithSimpleSchedule(x => {
                    var schedule = x.WithInterval(timespan);
                    if (_jobDescription.Schedule.RepeatCount > 0) {
                        schedule.WithRepeatCount(_jobDescription.Schedule.RepeatCount);
                    } else {
                        schedule.RepeatForever();
                    }
                }).StartNow();
            }

            return builder.Build();
        }

        private TimeSpan BuildInterval(IntervalUnit unit, long step) {
            switch (unit) {
                case IntervalUnit.Second:
                    return TimeSpan.FromSeconds(step);
                case IntervalUnit.Hour:
                    return TimeSpan.FromHours(step);
                case IntervalUnit.Minute:
                    return TimeSpan.FromMinutes(step);
                default:
                    return TimeSpan.FromMinutes(step);
            }
        }
    }
}
