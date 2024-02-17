using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Magicube.SSDP {
    public class Utils {
        public static IPAddress  UpnpMulticastV4Addr      = IPAddress.Parse("239.255.255.250");
        public static IPAddress  UpnpMulticastV6Addr1     = IPAddress.Parse("FF05::C"); // Site local
        public static IPAddress  UpnpMulticastV6Addr2     = IPAddress.Parse("FF02::C"); // Link local
        public static IPEndPoint UpnpMulticastV4EndPoint  = new IPEndPoint(UpnpMulticastV4Addr, 1900);
        public static IPEndPoint UpnpMulticastV6EndPoint1 = new IPEndPoint(UpnpMulticastV6Addr1, 1900);
        public static IPEndPoint UpnpMulticastV6EndPoint2 = new IPEndPoint(UpnpMulticastV6Addr2, 1900);

        private static bool MonoDetected = false;
        private static bool MonoActive = false;
        public static bool IsMono() {
            if (MonoDetected) return MonoActive;
            MonoActive = Type.GetType("Mono.Runtime") != null;
            MonoDetected = true;
            return MonoActive;
        }
    }
}
