using System;
using System.Collections.Generic;
using Polly;
using Polly.Retry;
using System.Linq;

namespace Magicube.GeoLocation {
    public class LocationServiceFactory {
        private readonly IList<ILocationService> _locationServices;
        public LocationServiceFactory(IEnumerable<ILocationService> locationServices) {
            _locationServices = locationServices.ToList();
        }

        public RetryPolicy Retry => Policy
            .Handle<Exception>()
            .Retry(_locationServices.Count, (ex, i,ctx) => {
                ctx["idx"] = i;
            });

        public GeoLocationResult TryGetLocation(string lat, string lng) {
            var ctx = new Context();
            ctx.Add("lat", lat);
            ctx.Add("lng", lng);
            return Retry.Execute(c => {
                var idx = 0;
                if (ctx.ContainsKey("idx"))
                    idx = (int)ctx["idx"];
                var result = _locationServices[idx].GetLocation(lat, lng).GetAwaiter().GetResult();
                if (result == null) throw new Exception("");
                return result;
            }, ctx);
        }
    }
}
