using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System.Collections.Generic;

namespace Magicube.Quartz.Scheduler {
    public class SchedulerConfigurationBuilder {
        public List<JobListenerConfiguration>     JobListeners           { get; }
        public List<CalendarConfiguration>        CalendarConfigurations { get; }
        public List<TriggerListenerConfiguration> TriggerListeners       { get; }

        public IServiceCollectionQuartzConfigurator SchedulerBuilder       { get; }
        public bool                                 UseDatabaseStore       { get; set; } = true;

        private readonly IServiceCollection _services;
        public SchedulerConfigurationBuilder(IServiceCollectionQuartzConfigurator schedulerBuilder, IServiceCollection services) {
            JobListeners           = new List<JobListenerConfiguration>();
            TriggerListeners       = new List<TriggerListenerConfiguration>();
            CalendarConfigurations = new List<CalendarConfiguration>();
            SchedulerBuilder       = schedulerBuilder;
            _services              = services;
            _services.AddSingleton<ContainerConfigurationProcessor>();
        }

        public SchedulerConfigurationBuilder UseMemoryStore() {
            UseDatabaseStore = false;
            SchedulerBuilder.UseInMemoryStore();
            return this;
        }

        public SchedulerConfigurationBuilder AddCalendar(
            string name,
            ICalendar calendar,
            bool replace,
            bool updateTriggers) {
            CalendarConfigurations.Add(new CalendarConfiguration(name, calendar, replace, updateTriggers));
            return this;
        }

        public SchedulerConfigurationBuilder AddTriggerListener<T>(params IMatcher<TriggerKey>[] matchers) where T : class, ITriggerListener {
            TriggerListeners.Add(new TriggerListenerConfiguration(typeof(T), matchers));
            _services.AddSingleton<ITriggerListener, T>();
            return this;
        }

        public SchedulerConfigurationBuilder AddJobListener<T>(params IMatcher<JobKey>[] matchers) where T : class, IJobListener {
            JobListeners.Add(new JobListenerConfiguration(typeof(T), matchers));
            _services.AddSingleton<IJobListener, T>();
            return this;
        }
    }
}
