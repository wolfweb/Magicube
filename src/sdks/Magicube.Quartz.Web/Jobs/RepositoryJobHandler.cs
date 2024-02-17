using Magicube.Quartz.Jobs;
using Magicube.Quartz.Web.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Magicube.Quartz.Web.Jobs {
    public class RepositoryJobHandler : JobBaseHandler {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public RepositoryJobHandler(
            ILogger logger,
            IStringLocalizer localizer,
            IOptions<JobOptions> options,
            IServiceScopeFactory serviceScopeFactory
            ) : base(logger, localizer, options) {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public override string JobType => L["数据操作"];

        public override Task ExecuteAsync() {
            var model = DataContext.TryGet<RepositoryModel>("Model");



            return Task.CompletedTask;
        }
    }
}
