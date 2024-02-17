using Magicube.Core;
using Magicube.Web.Environment.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Linq;
using Magicube.Modular.Web.Routing;
using WebApplicationBuilder = Magicube.Web.WebApplicationBuilder;

namespace Magicube.Modular.Web.Extensions {
    public static class WebApplicationBuilderExtension {
        public static WebApplicationBuilder UseModular(this WebApplicationBuilder builder) {
            var application = builder.ApplicationBuilder.ApplicationServices.GetService<Application>();

            var routes = builder.EndpointRouteBuilder;

            foreach (var modular in application.Modulars) {
                if (modular.Startup != null) {
                    ((IStartup)modular.Startup).Configure(builder.ApplicationBuilder, routes, application.ServiceProvider);
                }
                if (Directory.Exists(modular.StaticFileRoot)) {
                    builder.ApplicationBuilder.UseStaticFiles(new StaticFileOptions {
                        FileProvider = new PhysicalFileProvider(modular.StaticFileRoot),
                        RequestPath  = new PathString($"/{modular.Descriptor.Name}")
                    });
                }
            }
            
            var descriptors = builder.ApplicationBuilder.ApplicationServices.GetRequiredService<IActionDescriptorCollectionProvider>()
                    .ActionDescriptors.Items
                    .OfType<ControllerActionDescriptor>()
                    .ToArray();

            var mappers = builder.ApplicationBuilder.ApplicationServices.GetServices<IAreaControllerRouteMapper>().OrderBy(x => x.Order);

            foreach (var descriptor in descriptors) {
                if (!descriptor.RouteValues.ContainsKey("area")) {
                    continue;
                }

                foreach (var mapper in mappers) {
                    if (mapper.TryMapAreaControllerRoute(routes, descriptor)) {
                        break;
                    }
                }
            }
            
            return builder;
        }
    }
}
