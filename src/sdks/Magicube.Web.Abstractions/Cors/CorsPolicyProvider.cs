using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.Attributes;
using Magicube.Web.Sites;
using Magicube.Web.Attributes;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Magicube.Web {
    public class CorsPolicyProvider : ICorsPolicyProvider {
        private readonly ISiteManager _siteManager;
        private CorsOptions _corsOptions;
        public CorsPolicyProvider(IOptions<CorsOptions> options, ISiteManager siteManager) {
            _corsOptions = options.Value;
            _siteManager = siteManager;
        }
        public Task<CorsPolicy> GetPolicyAsync(HttpContext context, string policyName) {
            if (context == null) {
                throw new ArgumentNullException(nameof(context));
            }

            var policy = _corsOptions.GetPolicy(_corsOptions.DefaultPolicyName);
            if (policy != null) return Task.FromResult(policy);

            var policySetting = _siteManager.GetSite().As<CorsPolicySetting>();
            if (policySetting == null) return Task.FromResult<CorsPolicy>(null);

            var builder = new CorsPolicyBuilder();
            if (policySetting.AllowAnyHeader) {
                builder.AllowAnyHeader();
            } else if (policySetting.AllowedOrigins != null) {
                builder.WithHeaders(policySetting.AllowedOrigins);
            }
            if (policySetting.AllowAnyMethod) {
                builder.AllowAnyMethod();
            } else if (policySetting.AllowedMethods != null) {
                builder.WithMethods(policySetting.AllowedMethods);
            }
            if (policySetting.AllowAnyOrigin) {
                builder.AllowAnyOrigin();
            } else if (policySetting.AllowedOrigins != null) {
                builder.WithOrigins(policySetting.AllowedOrigins);
            }
            if (policySetting.AllowCredentials) {
                builder.AllowCredentials();
            } else {
                builder.DisallowCredentials();
            }
            return Task.FromResult(builder.Build());
        }
    }

    public class CorsPolicySetting {
        [NoUIRender]
        public string   Name             { get; set; } = "default";

        [Mutex("AllowedOrigins")]
        [Mutex("AllowCredentials", StatusType.Checked)]
        [Display(Name = "允许所有域名")]
        public bool     AllowAnyOrigin   { get; set; }

        [Display(Name = "指定域名")]
        public string[] AllowedOrigins   { get; set; }

        [Mutex("AllowedHeaders")]
        [Display(Name = "允许所有Header")]
        public bool     AllowAnyHeader   { get; set; }

        [Display(Name = "指定Header")]
        public string[] AllowedHeaders   { get; set; }

        [Mutex("AllowedMethods")]
        [Display(Name = "允许所有方法")]
        public bool     AllowAnyMethod   { get; set; }

        [Display(Name = "指定方法")]
        public string[] AllowedMethods   { get; set; }

        [Display(Name = "允许携带凭证")]
        [Mutex("AllowAnyOrigin", StatusType.Checked)]
        [Associated("AllowedOrigins")]
        public bool     AllowCredentials { get; set; }
    }
}
