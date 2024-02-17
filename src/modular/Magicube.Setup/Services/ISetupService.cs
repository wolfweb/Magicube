using Magicube.Core;
using Magicube.Data.Abstractions;
using Magicube.Eventbus;
using Magicube.Setup.Models;
using Magicube.Web;
using Magicube.Web.Events;
using Magicube.Web.Sites;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;

namespace Magicube.Setup.Services {
	public interface ISetupService {
        Task Execute(SetupContext ctx);
    }

    class SetupService: ISetupService {
        private readonly IEventProvider _eventProvider;
        private readonly ISiteManager _siteManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILookupNormalizer _lookupNormalizer;
        private readonly IHostApplicationLifetime _applicationLifetime;
        public SetupService(ISiteManager siteManager, IServiceProvider serviceProvider, IEventProvider eventProvider, ILookupNormalizer lookupNormalizer, IHostApplicationLifetime applicationLifetime) {
            _siteManager         = siteManager;
            _eventProvider       = eventProvider;
            _serviceProvider     = serviceProvider;
            _lookupNormalizer    = lookupNormalizer;
            _applicationLifetime = applicationLifetime;
        }
        /// <summary>
        /// 1、创建site信息
        /// 2、更新数据schema
        /// 3、准备基础数据
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public async Task Execute(SetupContext ctx) {
            ctx.Site.SupperUser = _lookupNormalizer.NormalizeName(ctx.Site.SupperUser);
            _siteManager.SaveSite(ctx.Site);

            var migration = _serviceProvider.GetService<IMigrationManagerFactory>().GetMigrationManager();
            migration.SchemaUp();

            ctx.Site.Status = SiteStatus.Running;
            _siteManager.UpdateSite(ctx.Site);

            await _eventProvider.OnSetupedAsync(new EventContext<ISetupContext>(ctx));
            _applicationLifetime.StopApplication();
            await Task.CompletedTask;
        }
    }
}
