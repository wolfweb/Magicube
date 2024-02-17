using AutoMapper;
using Magicube.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Magicube.AutoMap.AutoMapper {
    public static class ServiceCollectionExtensions {
        public static IServiceCollection AddAutoMap<Source, Target>(this IServiceCollection services)
            where Source : class
            where Target : class {
            services.Configure<AutoMapOptions>(options => options.RegisteModelMap<Source, Target>());
            return services;
        }

        public static IServiceCollection AddAutoMap<TProfile>(this IServiceCollection services) where TProfile : Profile {
            services.Configure<AutoMapOptions>(options => options.RegisteProfile<TProfile>());
            return services;
        }

        public static IServiceCollection AddAutoMap(this IServiceCollection services) {
            services.AddAutoMapper((sp, express) => {
                var options = sp.GetService<IOptions<AutoMapOptions>>();
                if (options != null && options.Value != null) {
                    foreach (var dict in options.Value.AuotMapModels) {
                        express.CreateMap(dict.Key, dict.Value);
                    }

                    foreach (var profile in options.Value.AutoProfiles) {
                        express.AddProfile(profile);
                    }
                }
            }, profileAssemblyMarkerTypes: Array.Empty<Type>(), ServiceLifetime.Scoped)
                .Replace<IMapperProvider, AutoMapperProvider>();
            return services;
        }
    }
}
