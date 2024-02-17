using Magicube.Core;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Magicube.Net {
    public static class CurlContextExtension {
        public static async Task<string> ReadAsString(this CurlContext ctx) {
            using (ctx.Response.Content)
            using (var respStream = await ctx.Response.Content.ReadAsStreamAsync())
            using (var reader = new StreamReader(respStream)) {
                var result = reader.ReadToEnd();
                Logger(ctx, result);
                return result;
            }
        }

        public static async Task<byte[]> ReadAsBytes(this CurlContext ctx) {
            using (ctx.Response.Content)
                return await ctx.Response.Content.ReadAsByteArrayAsync();
        }

        public static async Task<TResult> ReadAsStream<TResult>(this CurlContext ctx, Func<Stream, TResult> func) {
            using (ctx.Response.Content)
            using (var respStream = await ctx.Response.Content.ReadAsStreamAsync()) {
                return func(respStream);
            }
        }

        public static async Task<TResult> Read<TResult>(this CurlContext ctx, Func<String, TResult> func) {
            var res = await ReadAsString(ctx);
            return func(res);
        }

        public static async Task<TResult> ReadAs<TResult>(this CurlContext ctx) {
            return Json.Parse<TResult>(await ReadAsString(ctx));
        }

        private static void Logger(CurlContext ctx, string result) {
            ctx.Logger.LogTrace("{0} {1}\r\nBody:\t{2}\r\nResponse {3}",
                ctx.Request.Method,
                ctx.Request.RequestUri.AbsoluteUri,
                ctx.Body,
                result);
        }
    }
}
