using System;
using System.Linq;
using System.Net;
using System.Text;

namespace Magicube.WebServer {
    public sealed class Url : ICloneable {
        private string _basePath;
        private string _query;

        public Url() {
            Scheme   = Uri.UriSchemeHttp;
            HostName = string.Empty;
            Port     = null;
            BasePath = string.Empty;
            Path     = string.Empty;
            Query    = string.Empty;
        }

        public Url(string url) {
            var uri  = new Uri(url);
            HostName = uri.Host;
            Path     = uri.LocalPath;
            Port     = uri.Port;
            Query    = uri.Query;
            Scheme   = uri.Scheme;
        }

        public string Scheme   { get; set; }

        public string HostName { get; set; }

        public int?   Port     { get; set; }

        public string BasePath {
            get { return _basePath; }
            set {
                if (string.IsNullOrWhiteSpace(value))
                    return;

                _basePath = value.TrimEnd('/');
            }
        }

        public string Path { get; set; }
        public string Query {
            get { return _query; }
            set { _query = GetQuery(value); }
        }

        public string SiteBase {
            get {
                return new StringBuilder()
                    .Append(Scheme)
                    .Append(Uri.SchemeDelimiter)
                    .Append(GetHostName(HostName))
                    .Append(GetPort(Port))
                    .ToString();
            }
        }

        public bool IsSecure {
            get {
                return Uri.UriSchemeHttps.Equals(Scheme, StringComparison.OrdinalIgnoreCase);
            }
        }

        public override string ToString() {
            return new StringBuilder()
                .Append(Scheme)
                .Append(Uri.SchemeDelimiter)
                .Append(GetHostName(HostName))
                .Append(GetPort(Port))
                .Append(GetCorrectPath(BasePath))
                .Append(GetCorrectPath(Path))
                .Append(Query)
                .ToString();
        }

        object ICloneable.Clone() {
            return Clone();
        }

        public Url Clone() {
            return new Url {
                BasePath = BasePath,
                HostName = HostName,
                Port     = Port,
                Query    = Query,
                Path     = Path,
                Scheme   = Scheme
            };
        }

        public static implicit operator string(Url url) {
            return url.ToString();
        }

        public static implicit operator Url(string url) {
            return new Uri(url);
        }

        public static implicit operator Uri(Url url) {
            return new Uri(url.ToString(), UriKind.Absolute);
        }

        public static implicit operator Url(Uri uri) {
            return new Url {
                HostName = uri.Host,
                Path = uri.LocalPath,
                Port = uri.Port,
                Query = uri.Query,
                Scheme = uri.Scheme
            };
        }

        private static string GetQuery(string query) {
            return string.IsNullOrWhiteSpace(query) ? string.Empty : (query[0] == '?' ? query : '?' + query);
        }

        private static string GetCorrectPath(string path) {
            return (string.IsNullOrWhiteSpace(path) || path.Equals("/")) ? string.Empty : path;
        }

        private static string GetPort(int? port) {
            return port.HasValue ? string.Concat(":", port.Value) : string.Empty;
        }

        private static string GetHostName(string hostName) {
            IPAddress address;

            if (IPAddress.TryParse(hostName, out address)) {
                var addressString = address.ToString();

                return address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6
                    ? string.Format("[{0}]", addressString)
                    : addressString;
            }

            return hostName;
        }
    }

}
