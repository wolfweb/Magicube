using Microsoft.AspNetCore.Routing;

namespace Magicube.Autoroute.Abstractions {
    public class AutorouteOption {
        public string               ContentIdKey    { get; set; }
        public RouteValueDictionary AutorouteValues { get; set; } = new RouteValueDictionary();

    }
}
