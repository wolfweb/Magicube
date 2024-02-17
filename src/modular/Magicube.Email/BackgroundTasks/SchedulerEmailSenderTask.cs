using Magicube.Net.Email;
using Magicube.ScheduleBackgroundTask;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.Email.BackgroundTask {
    public class SchedulerEmailSenderTask : SchedulerBackgroundTask {
        public SchedulerEmailSenderTask(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory) { }

        public override async Task ProcessInScope(CancellationToken stoppingToken, IServiceProvider serviceProvider) {
            IEmailSender _emailSender = serviceProvider.GetService<IEmailSender>();
            ISchedulerMessageProvider _schedulerMessageProvider = serviceProvider.GetService<ISchedulerMessageProvider>();
            var _template = await _schedulerMessageProvider.Get();
            await _emailSender.SendAsync(_emailSender.Sender, _template.Subject, _template.Body);
        }
    }

    public interface ISchedulerMessageProvider {
        Task<ScheduleEmailMessageTemplate> Get();
    }

    public class NullSchedulerMessageProvider : ISchedulerMessageProvider {
        public Task<ScheduleEmailMessageTemplate> Get() {
            throw new System.NotImplementedException();
        }
    }

    public class ScheduleEmailMessageTemplate {
        public string Body    { get; set; }
        public string Subject { get; set; }
    }
}
