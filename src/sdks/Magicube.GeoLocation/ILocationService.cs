using System.Threading.Tasks;

namespace Magicube.GeoLocation {
    public interface ILocationService {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lat">纬度</param>
        /// <param name="lng">经度</param>
        /// <returns></returns>
        Task<GeoLocationResult> GetLocation(string lat, string lng);
    }

    public class GeoLocationResult {
        public string Address { get; set; }
        public string Name    { get; set; }
    }         
}
