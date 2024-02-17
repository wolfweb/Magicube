using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Magicube.WebServer {
    public static class Constants {
        public enum HttpStatusCode {
            Ok                      = 200,
            NoContent               = 204,
            MovedPermanently        = 301,
            FoundOrMovedTemporarily = 302,
            NotModified             = 304,
            Unauthorized            = 401,
            NotFound                = 404,
            InternalServerError     = 500
        }

        public static readonly string Version;

        public static readonly char DirectorySeparatorChar = Path.DirectorySeparatorChar;

        public static readonly string DirectorySeparatorString = Path.DirectorySeparatorChar.ToString();

        public static int    DefaultFileBufferSize = 1024 * 1024 * 4;

        public static string CorrelationIdRequestParameterName = "X-CorrelationId";

        public static string ElapsedMillisecondsResponseHeaderName = "X-Magicube-ElapsedMilliseconds";

        static Constants() {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            Version = fvi.FileVersion;
        }

        public static class CustomErrorResponse {
            public static string Unauthorized401 = "<html><head><title>Unauthorized</title></head><body><h3>Unauthorized: Error 401</h3><p>Oops, access is denied due to invalid credentials.</p></body></html>";

            public static string NotFound404 = "<html><head><title>Page Not Found</title></head><body><h3>Page Not Found: Error 404</h3><p>Oops, the page you requested was not found.</p></body></html>";

            public static string InternalServerError500 = "<html><head><title>Internal Server Error</title></head><body><h3>Internal Server Error: Error 500</h3><p>Oops, an internal error occurred.</p><!--ErrorMessage--></body></html>";
        }
    }

}
