using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicube.Quartz.ViewModels {
    public class JobViewModel {
        public JobViewModel() {
            JobInfos = new List<JobInfoViewModel>();
        }
        public string                 Group    { get; set; }
        
        public List<JobInfoViewModel> JobInfos { get; set; }
    }

    public class JobInfoViewModel {
        public string        Name            { get; set; }

        public DateTime?     NextAimTime     { get; set; }
                             
        public DateTime?     PreviousAimTime { get; set; }
                             
        public DateTime?     BeginTime       { get; set; }

        public DateTime?     EndTime         { get; set; }
                             
        public string        LastErrMsg      { get; set; }
                             
        public TriggerState  TriggerState    { get; set; }

        public string        Description     { get; set; }

        public string StateDisply {
            get {
                var state = string.Empty;
                switch (TriggerState) {
                    case TriggerState.Normal:
                        state = "正常";
                        break;
                    case TriggerState.Paused:
                        state = "暂停";
                        break;
                    case TriggerState.Complete:
                        state = "完成";
                        break;
                    case TriggerState.Error:
                        state = "异常";
                        break;
                    case TriggerState.Blocked:
                        state = "阻塞";
                        break;
                    case TriggerState.None:
                        state = "不存在";
                        break;
                    default:
                        state = "未知";
                        break;
                }
                return state;
            }
        }

        public string Interval       { get; set; }
    }
}
