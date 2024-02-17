using Microsoft.Extensions.Options;
using Quartz;
using Quartz.Spi;
using Quartz.Xml;
using System.Collections.Generic;

namespace Magicube.Quartz.Scheduler {
    public class ContainerConfigurationProcessor : XMLSchedulingDataProcessor {
        private readonly IOptions<QuartzOptions> options;

        public ContainerConfigurationProcessor(
            ITypeLoadHelper typeLoadHelper,
            IOptions<QuartzOptions> options)
            : base(typeLoadHelper) {
            this.options = options;
        }

        public override bool OverWriteExistingData => options.Value.Scheduling.OverWriteExistingData;
        public override bool IgnoreDuplicates => options.Value.Scheduling.IgnoreDuplicates;
        public override bool ScheduleTriggerRelativeToReplacedTrigger => options.Value.Scheduling.ScheduleTriggerRelativeToReplacedTrigger;

        protected override IReadOnlyList<IJobDetail> LoadedJobs => options.Value.JobDetails;
        protected override IReadOnlyList<ITrigger> LoadedTriggers => options.Value.Triggers;
    }
}
