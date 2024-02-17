using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Magicube.Core;

namespace Magicube.GeoLocation {
    public class BaiduLocationService : ILocationService {
        private const string url = @"http://api.map.baidu.com/geocoder/v2/?location={0}&output=json&ak={1}";

        private readonly GeoOptions _geoOptions;

        public BaiduLocationService(IOptions<GeoOptions> options) {
            _geoOptions = options.Value;
        }

        public async Task<GeoLocationResult> GetLocation(string lat, string lng) {
            string location = "{0},{1}".Format(lat, lng);
            string bdUrl = url.Format(location,_geoOptions.BaiduKey);
            using (HttpClient client = new HttpClient()) {
                string result = await client.GetStringAsync(bdUrl);
                var data = JObject.Parse(result);
                if (data["status"].ToString() == "0") {
                    string name = data["result"]["sematic_description"].ToString();
                    if (name.IsNullOrEmpty())
                        name = data["result"]["addressComponent"]["street"].ToString();

                    if(name.IsNullOrEmpty())
                        name = data["result"]["addressComponent"]["town"].ToString();

                    if (name.IsNullOrEmpty())
                        name = data["result"]["addressComponent"]["district"].ToString();

                    if (name.IsNullOrEmpty())
                        name = data["result"]["addressComponent"]["city"].ToString();

                    return new GeoLocationResult {
                        Address = data["result"]["formatted_address"].ToString(),
                        Name    = name
                    };
                }

                return null;
            }
        }
    }
}
