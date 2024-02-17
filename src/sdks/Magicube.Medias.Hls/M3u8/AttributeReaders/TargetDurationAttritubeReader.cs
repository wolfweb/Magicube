using System;
using System.Collections.Generic;

namespace Magicube.Medias.Hls {
    [M3U8Reader("#EXT-X-TARGETDURATION", typeof(TargetDurationAttritubeReader))]
    internal class TargetDurationAttritubeReader : IAttributeReader {
        public bool ShouldTerminate => false;

        public void Write(M3UFileInfo fileInfo, string value, IEnumerator<string> reader, Uri baseUri) {
            fileInfo.TargetDuration = To.Value<int>(value);
        }
    }
}