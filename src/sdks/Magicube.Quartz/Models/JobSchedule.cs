using Quartz;
using System;

namespace Magicube.Quartz.Models {
    [Serializable]
    public class JobSchedule : MarshalByRefObject {
        public JobSchedule(IntervalUnit interval, int intervalStep = 1) {            
            IntervalType = interval;
            IntervalStep = intervalStep;
        }
        public virtual  IntervalUnit     IntervalType { get; }
        public virtual  int              IntervalStep { get; set; } = 1;
        public virtual  ScheduleDateTime TimeOf       { get; set; }
        public virtual  int              RepeatCount  { get; set; } = -1;
    }
}
