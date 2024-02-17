using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Magicube.Core;

namespace Magicube.GeoLocation {
    public class AmapLocationService : ILocationService {
        private const string url = @"http://restapi.amap.com/v3/geocode/regeo?key={0}&location={1}";

        private readonly GeoOptions _geoOptions;

        public AmapLocationService(IOptions<GeoOptions> options) {
            _geoOptions = options.Value;
        }

        public async Task<GeoLocationResult> GetLocation(string lat, string lng) {
            string location = "{0},{1}".Format(lng, lat);
            string Url = url.Format(_geoOptions.AmapKey, location);
            using (var client = new HttpClient()) {
                string result = await client.GetStringAsync(Url);

                var data = JObject.Parse(result);
                if(data["status"].ToString()=="1"){
                    JToken neighborhood = data["regeocode"]["addressComponent"]["neighborhood"];

                    string name = neighborhood != null ? neighborhood["name"].ToString() : string.Empty;

                    if (name.IsNullOrEmpty() || name=="[]") {
                        name = data["regeocode"]["addressComponent"]["township"].ToString();
                    }

                    var address = data["regeocode"]["formatted_address"].ToString();
                    if (name != "[]" && address != "[]") {
                        return new GeoLocationResult {
                            Address = address,
                            Name = name
                        };
                    }
                }

                return null;
            }
        }
    }
}
