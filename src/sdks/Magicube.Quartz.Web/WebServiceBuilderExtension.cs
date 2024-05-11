using Magicube.Core;
using Magicube.Core.Modular;
using Magicube.Quartz;
using Magicube.Quartz.Scheduler;
using Magicube.Quartz.Web.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.AspNetCore;
using System;

namespace Magicube.Web {
    public static class WebServiceBuilderExtension {
        public static WebServiceBuilder AddQuartz(this WebServiceBuilder builder, Action<SchedulerConfigurationBuilder, IServiceCollectionQuartzConfigurator> handler = null) {
            builder.Services.Configure<ModularOptions>(options => {
                options.ModularShareAssembly.Add(typeof(MagicubeSchedulerFactory).Assembly);
            }).AddQuartzCore((builder, config) => {
                handler?.Invoke(builder, config);

                config.AddJob<HttpJobHandler>(jobConfig => {
                    jobConfig.StoreDurably();
                });

                config.AddJob<MailJobHandler>(jobConfig => {
                    jobConfig.StoreDurably();
                });
            }).Replace<ISchedulerFactory, MagicubeSchedulerFactory>()            
            .AddQuartzServer(options => {
                options.WaitForJobsToComplete = true;
            });
            return builder;
        }
    }
}
