using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Magicube.Core {
#if !DEBUG
using System.Diagnostics;
    [DebuggerStepThrough]
#endif
    public static class StreamExtensions {
        public static byte[] ReadAsBytes(this Stream stream) {
            using (var memoryStream = new MemoryStream()) {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public static async Task<byte[]> ReadAsBytesAsync(this Stream input, CancellationToken cancellationToken = default) {
            await using var ms = new MemoryStream();
            await input.CopyToAsync(ms, 16 * 1024, cancellationToken);
            return ms.ToArray();
        }

        public static async Task<string> ReadAsStringAsync(this Stream input, CancellationToken cancellationToken = default) {
            var bytes = await input.ReadAsBytesAsync(cancellationToken);
            return Encoding.UTF8.GetString(bytes);
        }

        public static void WriteAll(this Stream stream, byte[] datas) {
            stream.Write(datas, 0, datas.Length);
        }
    }
}
