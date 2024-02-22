using Magicube.Core;
using Magicube.Net.Email;
using Magicube.ScheduleBackgroundTask;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.Email.BackgroundTask {
    public class SchedulerEmailSenderTask : SchedulerBackgroundTask {
        public SchedulerEmailSenderTask(Application app) : base(app) { }

        public override async Task ProcessInScope(CancellationToken stoppingToken, IServiceScope scopeService) {
            IEmailSender _emailSender = scopeService.GetService<IEmailSender>();
            ISchedulerMessageProvider _schedulerMessageProvider = scopeService.GetService<ISchedulerMessageProvider>();
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
