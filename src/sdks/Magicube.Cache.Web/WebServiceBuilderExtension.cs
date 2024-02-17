using Magicube.Cache.Redis;
using Magicube.Cache.Web;
using System;

namespace Magicube.Web {
    public static class WebServiceBuilderExtension {
        public static WebServiceBuilder AddCache(this WebServiceBuilder builder, Action<RedisCacheBuilder> cacheBuilder = null) {
            builder.Services.AddCache(cacheBuilder);
            return builder;
        }
    }
}
