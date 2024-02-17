using Magicube.Core;
using Magicube.Web.Sites;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz;
using Quartz.Impl;
using Quartz.Util;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.Quartz.Scheduler {
    public class MagicubeSchedulerFactory : StdSchedulerFactory {
        private readonly SchedulerConfigurationBuilder _schedulerConfigurationBuilder;
        private readonly ContainerConfigurationProcessor _processor;
        private readonly IServiceProvider _serviceProvider;
        private readonly QuartzOptions _quartzOptions;
        private readonly ISiteManager _siteManager;
        private volatile int _initialize = 0;

        public MagicubeSchedulerFactory(
            IOptionsMonitor<QuartzOptions> options, 
            ISiteManager siteManager,
            IServiceProvider serviceProvider,
            ContainerConfigurationProcessor processor,
            SchedulerConfigurationBuilder schedulerConfigurationBuilder) {
            _quartzOptions                 = options.CurrentValue;
            _serviceProvider               = serviceProvider;
            _processor                     = processor;
            _schedulerConfigurationBuilder = schedulerConfigurationBuilder;
            _siteManager                   = siteManager;
        }

        public override async Task<IScheduler> GetScheduler(CancellationToken cancellationToken = default) {
            if (_schedulerConfigurationBuilder.UseDatabaseStore) {
                if (!UseDatabaseStorage()) {
                    Trace.WriteLine("站点未初始化，请初始化之后在重新启动。");
                    return null;
                }
            }

            base.Initialize(_quartzOptions.ToNameValueCollection());
            var scheduler = await base.GetScheduler(cancellationToken);

            if (Interlocked.CompareExchange(ref _initialize, 1, 0) == 0) {
                await InitializeScheduler(scheduler, cancellationToken);
            }

            return scheduler;
        }

        private async Task InitializeScheduler(IScheduler scheduler, CancellationToken cancellationToken) {
            foreach (var listener in _serviceProvider.GetServices<ISchedulerListener>()) {
                scheduler.ListenerManager.AddSchedulerListener(listener);
            }

            var jobListeners = _serviceProvider.GetServices<IJobListener>();
            foreach (var configuration in _schedulerConfigurationBuilder.JobListeners) {
                var listener = jobListeners.First(x => x.GetType() == configuration.ListenerType);
                scheduler.ListenerManager.AddJobListener(listener, configuration.Matchers);
            }

            var triggerListeners = _serviceProvider.GetServices<ITriggerListener>();
            foreach (var configuration in _schedulerConfigurationBuilder.TriggerListeners) {
                var listener = triggerListeners.First(x => x.GetType() == configuration.ListenerType);
                scheduler.ListenerManager.AddTriggerListener(listener, configuration.Matchers);
            }

            foreach (var configuration in _schedulerConfigurationBuilder.CalendarConfigurations) {
                await scheduler.AddCalendar(configuration.Name, configuration.Calendar, configuration.Replace, configuration.UpdateTriggers, cancellationToken);
            }

            await _processor.ScheduleJobs(scheduler, cancellationToken);
        }

        protected override T InstantiateType<T>(Type? implementationType) {
            var service = _serviceProvider.GetService<T>();
            if (service is null) {
                service = ObjectUtils.InstantiateType<T>(implementationType);
            }
            return service;
        }

        private bool UseDatabaseStorage() {
            var siteSettings = _siteManager.GetSite();
            if (siteSettings.Status != SiteStatus.Running) return false;
            var builder = SchedulerBuilder.Create(_quartzOptions.ToNameValueCollection());
            builder.UsePersistentStore(options => {
                switch (siteSettings.DatabaseProvider) {
                    case Data.MySql.ServiceCollectionExtensions.Identity:
                        options.UseMySql(siteSettings.ConnectionString);
                        break;
                    case Data.Sqlite.ServiceCollectionExtensions.Identity:
                        options.UseSQLite(siteSettings.ConnectionString);
                        break;
                    case Data.SqlServer.ServiceCollectionExtensions.Identity:
                        options.UseSqlServer(siteSettings.ConnectionString);
                        break;
                    case Data.PostgreSql.ServiceCollectionExtensions.Identity:
                        options.UsePostgres(siteSettings.ConnectionString);
                        break;
                    case Data.Mongodb.ServiceCollectionExtensions.Identity:
                        builder.UseInMemoryStore();
                        break;
                    default:
                        throw new NotSupportedException("unsupport database");
                }
            });

            return true;
        }
    }
}
