using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Senparc.CO2NET;
using Senparc.CO2NET.HttpUtility;
using Senparc.Weixin.Entities;
using System.Net;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Senparc.Weixin.AspNet;
using Senparc.Weixin.WxOpen;
using Magicube.Core;
using Magicube.LightApp.Abstractions;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Magicube.Data.Abstractions;

namespace Magicube.LightApp.Wechat {
    public static class ServiceCollectionExtension {
        /// <summary>
        /// 小程序服务注册
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddWechatOpen(this IServiceCollection services, Action<WechatOpenBuilder> builder = null) {
            SenparcDI.GlobalServiceCollection = services;
            services.ConfigureOptions<SenparcSettingSetup>();
            services.ConfigureOptions<SenparcWeixinSettingSetup>();
            services.AddHttpClient<SenparcHttpClient>()
                .ConfigurePrimaryHttpMessageHandler((IServiceProvider c) => {
                    return HttpClientHelper.GetHttpClientHandler(null, RequestUtility.SenparcHttpClientWebProxy, DecompressionMethods.GZip);
                });

            services.AddEntity<WechatUser, WechatUserMapping>();

            services.TryAddTransient<ILightAppUserService, WechatUserService>(WechatUserService.Identity);
            services.AddLightAppCore();

            var wxBuilder = new WechatOpenBuilder(services);

            builder?.Invoke(wxBuilder);

            return services;
        }
    }

    public static class ApplicationBuilderExtension {
        /// <summary>
        /// 小程序中间件注册
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseWechatOpen(this IApplicationBuilder app, IHostEnvironment env) {
            app.UseSenparcWeixin(env, null, null, register => { }, (register, settings) => {
                register.RegisterWxOpenAccount(settings);
            });
            return app;
        }
    }

    public class SenparcSettingSetup : IPostConfigureOptions<SenparcSetting> {
        public const string Key = "SenparcSetting";
        private readonly Application _app;
        public SenparcSettingSetup(Application app) {
            _app = app;
        }

        public void PostConfigure(string name, SenparcSetting options) {
            
        }
    }

    public class SenparcWeixinSettingSetup : IPostConfigureOptions<SenparcWeixinSetting> {
        public const string Key = "SenparcWeixinSetting";
        private readonly Application _app;
        public SenparcWeixinSettingSetup(Application app) {
            _app = app;
        }

        public void PostConfigure(string name, SenparcWeixinSetting options) {
            
        }
    }

    public class WechatOpenBuilder {
        private readonly IServiceCollection _services;

        public WechatOpenBuilder(IServiceCollection services) {
            _services = services;
        }

        public WechatOpenBuilder ConfigSenparcSetting(SenparcSetting settings) {
            _services.Configure<SenparcSetting>(x => { 
                x.IsDebug                       = settings.IsDebug;
                x.SenparcUnionAgentKey          = settings.SenparcUnionAgentKey;
                x.DefaultCacheNamespace         = settings.DefaultCacheNamespace;
                x.Cache_Redis_Configuration     = settings.Cache_Redis_Configuration;
                x.Cache_Memcached_Configuration = settings.Cache_Memcached_Configuration;
            });
            return this;
        }

        public WechatOpenBuilder ConfigSenparcWeixinSetting(SenparcWeixinSetting settings) {
            _services.Configure<SenparcWeixinSetting>(x => { 
                x.WxOpenAppId          = settings.WxOpenAppId;
                x.WxOpenToken          = settings.WxOpenToken;
                x.WxOpenAppSecret      = settings.WxOpenAppSecret;
                x.WxOpenEncodingAESKey = settings.WxOpenEncodingAESKey;
            });
            return this;
        }
    }
}
