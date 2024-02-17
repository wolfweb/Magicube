using Magicube.Core;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Magicube.AutoMap.Mapster {
    public static class ServiceCollectionExtensions {
        public static IServiceCollection AddAutoMap<Source, Target>(this IServiceCollection services, Action<TypeAdapterSetter<Source, Target>> callback)
            where Source : class
            where Target : class {
            services.Configure<AutoMapOptions>(options => {
                options.AddAutoMap(callback);
            });
            return services;
        }

        public static IServiceCollection AddAutoMapMapster(this IServiceCollection services) {
            services.Configure<AutoMapOptions>(options => { })
                .Replace<IMapperProvider, MapsterMapperProvider>()
                .AddScoped<IMapper, MagicubeServiceMapper>();
            return services;
        }
    }

    public class MagicubeServiceMapper : ServiceMapper {
        public MagicubeServiceMapper(IOptions<AutoMapOptions> options, IServiceProvider serviceProvider)
            : base(serviceProvider, options.Value.Config) {
        }
    }
}
