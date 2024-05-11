using Quartz;
using Quartz.Spi;
using System;

namespace Magicube.Quartz.Models {
    [Serializable]
    public class JobSchedule : MarshalByRefObject {
        public JobSchedule(IntervalUnit interval, int intervalStep = 1) {            
            IntervalType = interval;
            IntervalStep = intervalStep;
        }
        public virtual  IntervalUnit     IntervalType { get; }
        public virtual  int              IntervalStep { get; set; }
        public virtual  ScheduleDateTime DateTimeOf   { get; set; }
        public virtual  int              RepeatCount  { get; set; } = -1;

        public static implicit operator DateTimeOffset(JobSchedule schedule) {
            IMutableTrigger? trigger = null;
            if (schedule.IntervalType == IntervalUnit.Day) {
                var builder = CronScheduleBuilder.DailyAtHourAndMinute(schedule.DateTimeOf.Hour, schedule.DateTimeOf.Minute);
                trigger = builder.Build();
            }
            else if (schedule.IntervalType == IntervalUnit.Week) {
                if (schedule.DateTimeOf.DayOfWeek == null) throw new QuartzException("周计划任务请设置DayOfWeek");

                var builder = CronScheduleBuilder.WeeklyOnDayAndHourAndMinute(schedule.DateTimeOf.DayOfWeek.Value, schedule.DateTimeOf.Hour, schedule.DateTimeOf.Minute);
                trigger = builder.Build();
            }
            else if (schedule.IntervalType == IntervalUnit.Month) {
                var builder = CronScheduleBuilder.MonthlyOnDayAndHourAndMinute(schedule.DateTimeOf.Day, schedule.DateTimeOf.Hour, schedule.DateTimeOf.Minute);
                trigger = builder.Build();
            }

            return trigger?.GetFireTimeAfter(trigger.StartTimeUtc) ?? DateTimeOffset.UtcNow;
        }
    }
}
