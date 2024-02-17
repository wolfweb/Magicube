using Cysharp.Text;
using Magicube.Core;
using Magicube.Quartz.Jobs;
using Magicube.Quartz.Services;
using Magicube.Web;
using Magicube.Web.Sites;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Quartz;
using Quartz.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;
using LogLevel = Quartz.Logging.LogLevel;

namespace Magicube.Schedule.Test {
    public class SchedulerQuartzTest {
        private readonly IServiceProvider _serviceProvider;
        public SchedulerQuartzTest() {
            var siteMock = new Mock<ISiteManager>();

            siteMock.Setup(x => x.GetSite()).Returns(new DefaultSite());

            var services = new ServiceCollection()
               .AddLogging()
               .AddSingleton<ISiteManager>(x => siteMock.Object);

            var webBuilder = new WebServiceBuilder(services);
            webBuilder.AddQuartz((builder,config) => {
                builder.UseMemoryStore();
            });
            
            _serviceProvider = services.BuildServiceProvider();

            LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());
        }

        [Fact]
        public async Task Func_Scheduler_Test() {
            var scheduleFactory = _serviceProvider.GetService<ISchedulerFactory>();
            Assert.NotNull(scheduleFactory);
            var scheduler = await scheduleFactory.GetScheduler();
            Assert.NotNull(scheduler);
            
            //await scheduler.Start();

            var jobHostService = _serviceProvider.GetService<IJobService>();
            await jobHostService.RegisterJob<FooJob>();
            await jobHostService.AddTrigger<FooJob>(DateTime.Now.AddMinutes(15), builder => {
                
            });

            Assert.True(await jobHostService.ExistJob<FooJob>());
            Assert.True(await jobHostService.ExistTrigger<FooJob>());

            await jobHostService.RemoveJob<FooJob>();

            Assert.True(!await jobHostService.ExistJob<FooJob>());
            Assert.True(!await jobHostService.ExistTrigger<FooJob>());


            var jobKey = "Abc";
            await jobHostService.RegisterJob<FooJob>(jobKey);
            await jobHostService.AddTrigger<FooJob>(jobKey, DateTime.Now.AddMinutes(15), builder => {

            });

            Assert.True(await jobHostService.ExistJob<FooJob>(jobKey));
            Assert.True(await jobHostService.ExistTrigger<FooJob>(jobKey));

            await jobHostService.RemoveJob<FooJob>(jobKey);

            Assert.True(!await jobHostService.ExistJob<FooJob>(jobKey));
            Assert.True(!await jobHostService.ExistTrigger<FooJob>(jobKey));
        }
    }

    public class FooJob : JobBaseHandler {
        public FooJob(ILogger<FooJob> logger, IOptions<JobOptions> options) : base(logger, null, options) {
        }

        public override string JobType => "Foo";

        public override Task ExecuteAsync() {
            Trace.WriteLine("job running");
            return Task.CompletedTask;
        }
    }

    class ConsoleLogProvider : ILogProvider {
        public Logger GetLogger(string name) {
            return (level, func, exception, parameters) =>
            {
                if (level >= LogLevel.Info && func != null) {
                    Trace.WriteLine(ZString.Format($"[{DateTime.Now.ToLongTimeString() }] [{level}] " + func(), parameters));
                }
                return true;
            };
        }

        public IDisposable OpenNestedContext(string message) {
            throw new NotImplementedException();
        }

        public IDisposable OpenMappedContext(string key, object value, bool destructure = false) {
            throw new NotImplementedException();
        }
    }

}
