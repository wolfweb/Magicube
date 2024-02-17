using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;

namespace Magicube.Web.Environment.Builder {
    public class MagicubeCoreBuilder {
        private ConcurrentDictionary<int, StartupActions> _actions { get; } = new ConcurrentDictionary<int, StartupActions>();
        public IServiceCollection ApplicationServices { get; }

        public MagicubeCoreBuilder(IServiceCollection services) {
            ApplicationServices = services;
        }

        public MagicubeCoreBuilder RegisterStartup<T>() where T : class, IStartup {
            ApplicationServices.AddTransient<IStartup, T>();
            return this;
        }

        public MagicubeCoreBuilder ConfigureServices(Action<IServiceCollection, IServiceProvider> configure, int order = 0) {
            if (!_actions.TryGetValue(order, out var actions)) {
                actions = _actions[order] = new StartupActions(order);

                ApplicationServices.AddTransient<IStartup>(sp => new StartupActionsStartup(
                    sp.GetRequiredService<IServiceProvider>(), actions, order));
            }

            actions.ConfigureServicesActions.Add(configure);

            return this;
        }

        public MagicubeCoreBuilder ConfigureServices(Action<IServiceCollection> configure, int order = 0) {
            return ConfigureServices((s, sp) => configure(s), order);
        }

        public MagicubeCoreBuilder Configure(Action<IApplicationBuilder, IEndpointRouteBuilder, IServiceProvider> configure, int order = 0) {
            if (!_actions.TryGetValue(order, out var actions)) {
                actions = _actions[order] = new StartupActions(order);

                ApplicationServices.AddTransient<IStartup>(sp => new StartupActionsStartup(
                    sp.GetRequiredService<IServiceProvider>(), actions, order));
            }

            actions.ConfigureActions.Add(configure);

            return this;
        }

        public MagicubeCoreBuilder Configure(Action<IApplicationBuilder, IEndpointRouteBuilder> configure, int order = 0) {
            return Configure((app, routes, sp) => configure(app, routes), order);
        }

        public MagicubeCoreBuilder Configure(Action<IApplicationBuilder> configure, int order = 0) {
            return Configure((app, routes, sp) => configure(app), order);
        }
    }
}
