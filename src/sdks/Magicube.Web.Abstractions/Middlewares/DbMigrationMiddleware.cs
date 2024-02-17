using Magicube.Core.Signals;
using Magicube.Data.Abstractions;
using Magicube.Data.Migration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Magicube.Web.Middlewares {
    public class DbMigrationMiddleware {
        private readonly ISignal _signal;
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public DbMigrationMiddleware(RequestDelegate next, ISignal signal, IServiceScopeFactory serviceScopeFactory) {
            _next                = next;
            _signal              = signal;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task Invoke(HttpContext httpContext) {
            var (exist, changeToken) = _signal.GetToken(MigrationConstant.MigrationSignalKey);

            if (!exist) {
                //todo ： 非线程安全，
                changeToken.RegisterChangeCallback(m => {
                    using (var scoped = _serviceScopeFactory.CreateScope()) {
                        var migration = scoped.ServiceProvider.GetRequiredService<IMigrationManagerFactory>().GetMigrationManager();
                        migration.SchemaUp();
                    }
                }, null);
            }

            return _next.Invoke(httpContext);
        }
    }
}
