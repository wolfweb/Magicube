using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Magicube.Web.Environment.Builder {
    public class StartupBase : IStartup {
        public virtual int Order { get; } = 0;

        public virtual void ConfigureServices(IServiceCollection services) {}

        public virtual void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider) {}
    }
}
