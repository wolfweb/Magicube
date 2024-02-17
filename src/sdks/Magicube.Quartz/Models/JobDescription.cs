using Magicube.Core;
using System;

namespace Magicube.Quartz.Models {
    [Serializable]
    public class JobDescriptor {
        public JobDescriptor() {
            JobData = new TransferContext();
        }
        public string          JobGroup                    { get; set; }
        public string          JobName                     { get; set; }
        public string          Description                 { get; set; }
        public JobSchedule     Schedule                    { get; set; }
        public TransferContext JobData                     { get; }
    }
}
