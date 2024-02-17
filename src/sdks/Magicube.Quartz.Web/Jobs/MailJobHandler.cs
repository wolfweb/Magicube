using Magicube.Net.Email;
using Magicube.Quartz.Jobs;
using Magicube.Quartz.Web.Models;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Magicube.Quartz.Web.Jobs {
    public class MailJobHandler : JobBaseHandler {
        private readonly IEmailSender _emailSender;

        public MailJobHandler(ILogger logger, 
            IStringLocalizer<MailJobHandler> localizer, 
            IOptions<JobOptions> options, 
            IEmailSender emailSender) : base(logger, localizer, options) {
            _emailSender = emailSender;
        }

        public override string JobType => L["发送邮件"];

        public override async Task ExecuteAsync() {
            var model = DataContext.TryGet<SendMailModel>("Model");
            if (model == null) {
                Logger.LogWarning("mail job request send mail model");
                return;
            }
            await _emailSender.SendAsync(model.Receiver, model.Body, model.Body);
        }
    }
}
