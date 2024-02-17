using System;
using System.Collections.Generic;

namespace Magicube.Medias.Hls {
    [M3U8Reader("#EXT-X-VERSION", typeof(VersionAttributeReader))]
    internal class VersionAttributeReader : IAttributeReader {
        public bool ShouldTerminate => false;

        public void Write(M3UFileInfo fileInfo, string value, IEnumerator<string> reader, Uri baseUri) {
            fileInfo.Version = To.Value<int>(value);
        }
    }
}