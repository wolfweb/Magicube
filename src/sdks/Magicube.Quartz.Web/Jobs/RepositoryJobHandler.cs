using Magicube.Core;
using Magicube.Quartz.Jobs;
using Magicube.Quartz.Web.Models;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Magicube.Quartz.Web.Jobs {
    public class RepositoryJobHandler : JobBaseHandler {
        private readonly Application _app;
        public RepositoryJobHandler(
            ILogger logger,
            Application app,
            IStringLocalizer localizer,
            IOptions<JobOptions> options
            ) : base(logger, localizer, options) {
            _app = app;
        }

        public override string JobType => L["数据操作"];

        public override Task ExecuteAsync() {
            var model = DataContext.TryGet<RepositoryModel>("Model");



            return Task.CompletedTask;
        }
    }
}
