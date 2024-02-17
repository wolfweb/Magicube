using System;
using System.Collections.Generic;

namespace Magicube.Medias.Hls {
    [M3U8Reader("#EXT-X-DISCONTINUITY", typeof(DiscontinuityAttributeReader))]
    internal class DiscontinuityAttributeReader : IAttributeReader {
        public bool ShouldTerminate => false;

        public void Write(M3UFileInfo m3UFileInfo, string value, IEnumerator<string> reader, Uri baseUri) {
        }
    }
}