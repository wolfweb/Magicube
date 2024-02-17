using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Magicube.Web {
    public class WildcardCorsService : CorsService {
        public WildcardCorsService(IOptions<CorsOptions> options, ILoggerFactory loggerFactory)
            : base(options, loggerFactory) {
        }

        public override void EvaluateRequest(HttpContext context, CorsPolicy policy, CorsResult result) {
            var origin = context.Request.Headers[CorsConstants.Origin];
            EvaluateOriginForWildcard(policy.Origins, origin);
            base.EvaluateRequest(context, policy, result);
        }

        public override void EvaluatePreflightRequest(HttpContext context, CorsPolicy policy, CorsResult result) {
            var origin = context.Request.Headers[CorsConstants.Origin];
            EvaluateOriginForWildcard(policy.Origins, origin);
            base.EvaluatePreflightRequest(context, policy, result);
        }

        private void EvaluateOriginForWildcard(IList<string> origins, string origin) {
            if (!origins.Contains(origin)) {
                var wildcardDomains = origins.Where(o => o.StartsWith("*") || o.EndsWith("*"));
                if (wildcardDomains.Any()) {
                    foreach (var wildcardDomain in wildcardDomains) {
                        if (wildcardDomain.StartsWith("*") && origin.EndsWith(wildcardDomain.Substring(1))) {
                            origins.Add(origin);
                            break;
                        } else if (wildcardDomain.EndsWith("*") && new Uri(origin).Host.StartsWith(wildcardDomain.Substring(0, wildcardDomain.IndexOf('*')))) {
                            origins.Add(origin);
                            break;
                        }
                    }
                }
            }
        }
    }
}
