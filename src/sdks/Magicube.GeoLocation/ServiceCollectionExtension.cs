using Microsoft.Extensions.DependencyInjection;
using Magicube.Core;

namespace Magicube.GeoLocation {
    public static  class ServiceCollectionExtension {
        public static IServiceCollection AddGeoService(this IServiceCollection services) {
            services.Configure<GeoOptions>(options => {
                if (options.TencentKey.IsNullOrEmpty())
                    options.TencentKey = "CJ5BZ-LCA3V-CQGPP-UAP5J-YUCG2-GCFWI";
                if (options.AmapKey.IsNullOrEmpty())
                    options.AmapKey = "8716220aa2d9a1c956d67266a40d414e";

                if (options.BaiduKey.IsNullOrEmpty())
                    options.BaiduKey = "WEc8RlPXzSifaq9RHxE1WW7lRKgbid6Y";
            });

            services.AddTransient<ILocationService, AmapLocationService>()
                    .AddTransient<ILocationService, TencentLocationService>()
                    .AddTransient<ILocationService, BaiduLocationService>()
                    .AddTransient<LocationServiceFactory>();
            return services;
        }
    }
}
