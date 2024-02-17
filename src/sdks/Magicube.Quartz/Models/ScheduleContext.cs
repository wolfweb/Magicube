using Magicube.Core;
using System;

namespace Magicube.Quartz.Models {
    public class ScheduleContext {
        public ScheduleContext() {
            Data = new TransferContext();
        }
        public TransferContext  Data            { get; set; }
        public DateTimeOffset   StartAt         { get; set; }
    }
}
