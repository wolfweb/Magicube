using Magicube.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Magicube.Web {
    public static class HttpRequestExtension {
        public static Uri ToUri(this HttpRequest request) {
            return new Uri(request.GetEncodedUrl());
        }

        public static Uri ToAbsoluteUrl(this HttpRequest request, string relativePath) {
            var absoluteUrl = $"{request.Scheme}://{request.Host}{relativePath}";
            return new Uri(absoluteUrl, UriKind.Absolute);
        }

        public static Task<string> ReadAsStringAsync(this HttpRequest request) {
            using (var reader = new StreamReader(request.Body)) {
                return reader.ReadToEndAsync();
            }
        }

        public static async Task<byte[]> ReadAsBytesAsync(this HttpRequest request) {
            if (request.Body == null || request.Body == Stream.Null)
                return null;

            request.EnableBuffering();

            var content = await request.Body.ReadAsBytesAsync();
            request.Body.Seek(0, SeekOrigin.Begin);
            return content;
        }

        public static bool IsApiRequest(this HttpRequest request) => request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase);
    }
}
