using Magicube.Core;
using Magicube.Core.Signals;
using Magicube.Data.Abstractions;
using Magicube.Data.Migration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Magicube.Web.Middlewares {
    public class DbMigrationMiddleware {
        private readonly ISignal _signal;
        private readonly Application _app;
        private readonly RequestDelegate _next;
        public DbMigrationMiddleware(RequestDelegate next, ISignal signal, Application app) {
            _app    = app;
            _next   = next;
            _signal = signal;
        }

        public Task Invoke(HttpContext httpContext) {
            var (exist, changeToken) = _signal.GetToken(MigrationConstant.MigrationSignalKey);

            if (!exist) {
                //todo ： 非线程安全，
                changeToken.RegisterChangeCallback(m => {
                    using (var scoped = _app.CreateScope()) {
                        var migration = scoped.GetService<IMigrationManagerFactory>().GetMigrationManager();
                        migration.SchemaUp();
                    }
                }, null);
            }

            return _next.Invoke(httpContext);
        }
    }
}
