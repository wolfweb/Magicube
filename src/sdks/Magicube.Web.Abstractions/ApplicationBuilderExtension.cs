using Magicube.Core;
using Magicube.Web.Middlewares;
using Microsoft.AspNetCore.Builder;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Magicube.Web.Sites;

namespace Magicube.Web {
    public static class ApplicationBuilderExtension {
        public static IApplicationBuilder UseWeb(this IApplicationBuilder app, Action<WebApplicationBuilder> action = null) {
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMiddleware<PoweredByMiddleware>();
            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseMiddleware<DbMigrationMiddleware>();

            InitializeAppLifetimeEvent(app);

            app.UseConf();

            var application = app.ApplicationServices.GetService<Application>();
            if (application.ServiceProvider == null) {
                application.ServiceProvider = app.ApplicationServices;
            }

            app.UseStatusCodePagesWithReExecute("/Error", "?code={0}");

            app.UseRouting();
            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                action?.Invoke(new WebApplicationBuilder(app, endpoints));
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });

            return app;
        }

        private static void InitializeAppLifetimeEvent(IApplicationBuilder app) {
            var appLifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();
            var siteManager = app.ApplicationServices.GetService<ISiteManager>();

            appLifetime.ApplicationStarted.Register(() => {
                
            });

            appLifetime.ApplicationStopped.Register(() => {
                if(siteManager.GetSite()?.Status == SiteStatus.Running) {

                }
            });
        }
    }
}
