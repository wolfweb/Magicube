using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Magicube.Net {
    internal class FackHttpContent : HttpContent {
        private static readonly Stream EmptyStream = new MemoryStream(Array.Empty<byte>());

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context) {
            EmptyStream.CopyTo(stream);
            await Task.CompletedTask;
        }

        protected override bool TryComputeLength(out long length) {
            length = 0;
            return true;
        }
    }

}
