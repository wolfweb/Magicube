using System;
using System.Collections.Generic;

namespace Magicube.Medias.Hls {
    [M3U8Reader("#EXT-X-MEDIA-SEQUENCE", typeof(SequenceAttributeReader))]
    internal class SequenceAttributeReader : IAttributeReader {
        public bool ShouldTerminate => false;

        public void Write(M3UFileInfo fileInfo, string value, IEnumerator<string> reader, Uri baseUri) {
            fileInfo.MediaSequence = To.Value<int>(value);
        }
    }
}