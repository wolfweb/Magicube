using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Magicube.Web.Environment.Builder {
    internal class StartupActions {
        public StartupActions(int order) {
            Order = order;
        }

        public int Order { get; }

        public ICollection<Action<IServiceCollection, IServiceProvider>> ConfigureServicesActions { get; } = new List<Action<IServiceCollection, IServiceProvider>>();

        public ICollection<Action<IApplicationBuilder, IEndpointRouteBuilder, IServiceProvider>> ConfigureActions { get; } = new List<Action<IApplicationBuilder, IEndpointRouteBuilder, IServiceProvider>>();
    }
}
