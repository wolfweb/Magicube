using Magicube.Core;
using Magicube.Data.Abstractions;
using Magicube.Executeflow.Activities;
using Magicube.Executeflow.Configurations;
using Magicube.Executeflow.Entities;
using Magicube.Executeflow.Scripting;
using Magicube.Executeflow.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Magicube.Executeflow {
    public static class ServiceCollectionExtension {
        public static IServiceCollection AddExecuteflow(this IServiceCollection services) {
            RegisterEntities(services);

            AddActivities(services);

            services
                .AddSingleton<IScriptingEngine, JavaScriptEngine>()
                .AddSingleton<IActivityProvider, ActivityProvider>()
                .AddSingleton<ExecuteflowService>()
                .AddScoped<IExecuteflowContextHandler, DefaultExecuteflowContextHandler>()
                .AddScoped<EvaluatorFactory>()
                ;

            return services;
        }

        public static IServiceCollection AddActivity<T>(this IServiceCollection services) where T: Activity{
            services.Configure<ExecuteflowOptions>(x => x.RegisterActivity(typeof(T)));
            return services;
        }

        private static void AddActivities(IServiceCollection services) {
            services.AddActivity<IfElse>()
                .AddActivity<For>()
                .AddActivity<While>()
                .AddActivity<ForEach>()
                .AddActivity<SetInput>()
                .AddActivity<SetOutput>()
                .AddActivity<SetProperty>()
                ;
        }

        private static void RegisterEntities(IServiceCollection services) {
            services
                .AddEntity<ActivityStore, ActivityEntityMapping>()
                .AddEntity<ExecuteflowStore>()
                .AddEntity<ExecuteflowStateStore, ExecuteflowStateEntityMapping>();
        }
    }
}
