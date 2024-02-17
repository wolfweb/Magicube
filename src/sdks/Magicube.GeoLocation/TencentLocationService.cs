using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Magicube.Core;

namespace Magicube.GeoLocation {
    public class TencentLocationService: ILocationService {
        public const string url = @"http://apis.map.qq.com/ws/geocoder/v1/?key={0}&location={1}&get_poi=1&poi_options=address_format%3Dshort";

        private readonly GeoOptions _geoOptions;

        public TencentLocationService(IOptions<GeoOptions> options) {
            _geoOptions = options.Value;
        }

        public async Task<GeoLocationResult> GetLocation(string lat, string lng) {
            string location = "{0},{1}".Format(lat, lng);
            string bdUrl = url.Format(_geoOptions.TencentKey, location);
            using (HttpClient client = new HttpClient()) {
                var result = await client.GetStringAsync(bdUrl);
                var data = JObject.Parse(result);
                if (data["status"].ToString() == "0") {
                    JToken landmark = null,adressReference =null;
                    if (data["result"]["address_reference"] != null) {
                        adressReference = data["result"]["address_reference"];
                        landmark = adressReference["landmark_l2"];
                    }

                    string name = string.Empty;
                    if (landmark != null)
                        name = landmark["title"].ToString();

                    if (name.IsNullOrEmpty()) {
                        name = adressReference != null ? adressReference["town"]["title"].ToString() : string.Empty;
                    }

                    if (name.IsNullOrEmpty()) {
                        name = data["result"]["address_component"]["locality"].ToString();
                    }

                    return new GeoLocationResult {
                        Address = data["result"]["address"].ToString(),
                        Name    = name
                    };
                }
                return null;
            }
        }
    }
}
