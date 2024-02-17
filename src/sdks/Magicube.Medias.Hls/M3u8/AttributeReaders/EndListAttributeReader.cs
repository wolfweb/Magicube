using System;
using System.Collections.Generic;

namespace Magicube.Medias.Hls {
    [M3U8Reader("#EXT-X-ENDLIST", typeof(EndListAttributeReader))]
    internal class EndListAttributeReader : IAttributeReader {
        public bool ShouldTerminate => true;

        public void Write(M3UFileInfo m3UFileInfo, string value, IEnumerator<string> reader, Uri baseUri) {
            m3UFileInfo.PlaylistType = "VOD";
        }
    }
}