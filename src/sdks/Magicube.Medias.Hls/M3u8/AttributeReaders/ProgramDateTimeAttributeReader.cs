using System;
using System.Collections.Generic;

namespace Magicube.Medias.Hls {
    [M3U8Reader("#EXT-X-PROGRAM-DATE-TIME", typeof(ProgramDateTimeAttributeReader))]
    internal class ProgramDateTimeAttributeReader : IAttributeReader {
        public bool ShouldTerminate => false;

        public void Write(M3UFileInfo fileInfo, string value, IEnumerator<string> reader, Uri baseUri) {
            fileInfo.ProgramDateTime = To.Value<DateTime>(value);
        }
    }
}