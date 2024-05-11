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

            if (_jobDescription.Schedule.DateTimeOf != null) {
                int hour = _jobDescription.Schedule.DateTimeOf.Hour,
                    minute = _jobDescription.Schedule.DateTimeOf.Minute,
                    second = _jobDescription.Schedule.DateTimeOf.Second,
                    day = _jobDescription.Schedule.DateTimeOf.Day;

                if (_jobDescription.Schedule.RepeatCount > 0) {
                    if (_jobDescription.Schedule.IntervalType == IntervalUnit.Month || _jobDescription.Schedule.IntervalType == IntervalUnit.Week || _jobDescription.Schedule.IntervalType == IntervalUnit.Day) {
                        builder.WithSimpleSchedule(x => {
                            x.WithRepeatCount(_jobDescription.Schedule.RepeatCount);
                        }).StartAt(_jobDescription.Schedule);
                    }
                    else {
                        builder.WithDailyTimeIntervalSchedule(x => {
                            x.WithRepeatCount(_jobDescription.Schedule.RepeatCount)
                            .WithIntervalInHours(24)
                            .OnEveryDay()
                            .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(hour, minute));

                            if (_jobDescription.Schedule.DateTimeOf.DayOfWeek.HasValue) {
                                x.OnDaysOfTheWeek(_jobDescription.Schedule.DateTimeOf.DayOfWeek.Value);
                            }
                        }).StartNow();
                    }
                }
                else {
                    if (_jobDescription.Schedule.IntervalType == IntervalUnit.Month) {
                        builder.WithSchedule(CronScheduleBuilder.MonthlyOnDayAndHourAndMinute(_jobDescription.Schedule.DateTimeOf.Day, hour, minute)).StartNow();
                    }
                    else if (_jobDescription.Schedule.IntervalType == IntervalUnit.Week) {
                        if (_jobDescription.Schedule.DateTimeOf.DayOfWeek == null) throw new QuartzException("周计划任务请设置DayOfWeek");
                        builder.WithSchedule(CronScheduleBuilder.WeeklyOnDayAndHourAndMinute(_jobDescription.Schedule.DateTimeOf.DayOfWeek.Value, hour, minute)).StartNow();
                    }
                    else if (_jobDescription.Schedule.IntervalType == IntervalUnit.Day) {
                        builder.WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(hour, minute)).StartNow();
                    }
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
                case IntervalUnit.Minute:
                    return TimeSpan.FromMinutes(step);
                case IntervalUnit.Hour:
                    return TimeSpan.FromHours(step);
                default:
                    return TimeSpan.FromMinutes(step);
            }
        }
    }
}
