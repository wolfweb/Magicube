using Magicube.Core;
using Magicube.Data.Abstractions;
using Magicube.Quartz.Jobs;
using Magicube.Quartz.Scheduler;
using Magicube.Quartz.Services;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;

namespace Magicube.Quartz {
    public static class ServiceCollectionExtension {
        public static IServiceCollection AddQuartzCore(this IServiceCollection services, Action<SchedulerConfigurationBuilder, IServiceCollectionQuartzConfigurator> action = null) {
            services.AddQuartz(config => {
                var builder = new SchedulerConfigurationBuilder(config, services);
                config.InterruptJobsOnShutdown = true;
                config.InterruptJobsOnShutdownWithWait = true;
                config.MaxBatchSize = 5;
                config.UseDefaultThreadPool(maxConcurrency: 10);

                action?.Invoke(builder, config);

                services.AddSingleton(builder);
            })
            .AddScoped<IJobService, JobService>()
            .Configure<JobOptions>(x => { 

            });

            AddEntities(services);

            return services;
        }

        private static void AddEntities(IServiceCollection services) {
            services.AddEntity<QuartzLocks            , QuartzLocksMapping>();
            services.AddEntity<QuartzTriggers         , QuartzTriggersMapping>();
            services.AddEntity<QuartzCalendars        , QuartzCalendarsMapping>();
            services.AddEntity<QuartzJobDetails       , QuartzJobDetailsMapping>();
            services.AddEntity<QuartzBlobTriggers     , QuartzBlobTriggersMapping>();
            services.AddEntity<QuartzCronTriggers     , QuartzCronTriggersMapping>();
            services.AddEntity<QuartzFiredTriggers    , QuartzFiredTriggersMapping>();
            services.AddEntity<QuartzSchedulerState   , QuartzSchedulerStateMapping>();
            services.AddEntity<QuartzSimpleTriggers   , QuartzSimpleTriggersMapping>();
            services.AddEntity<QuartzSimpropTriggers  , QuartzSimpropTriggersMapping>();
            services.AddEntity<QuartzPausedTriggerGrps, QuartzPausedTriggerGrpsMapping>();
        }
    }
}
