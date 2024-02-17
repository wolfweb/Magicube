using Magicube.Core;
using Magicube.Core.IO;
using Magicube.RetryTask;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly.Retry;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Magicube.Quartz.Jobs {
    public abstract class JobBaseHandler : IJob {
        private readonly JobOptions _jobOptions;
        private readonly AsyncRetryPolicy _retry;

        protected readonly ILogger Logger;
        protected readonly IStringLocalizer L;

        public JobBaseHandler(ILogger logger, IStringLocalizer localizer, IOptions<JobOptions> options) {
            L           = localizer;
            Logger      = logger;
            _jobOptions = options.Value;
            _retry      = new RetryTaskBuilder<RetryContext>(ctx => {
                Logger.LogError(ctx.LastError, "Error:");
            }, _jobOptions.RetryTimes, _jobOptions.WaitSecond).BuildAsync();
        }

        public abstract string JobType { get; }

        public async Task Execute(IJobExecutionContext context) {
            var desc = context.MergedJobDataMap["Desc"];
            if (desc != null) ConsoleUtil.Info($"Start Job {desc} At {DateTime.Now}");

            DataContext = context.MergedJobDataMap["DataContext"] as TransferContext;
            await _retry.ExecuteAsync(async () => {
                await ExecuteAsync();
            });
        }

        public abstract Task ExecuteAsync();

        protected TransferContext DataContext { get; private set; }
    }
}
