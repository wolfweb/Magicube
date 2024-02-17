using System;
using System.Collections.Generic;

namespace Magicube.Medias.Hls {
    internal abstract class AttributeReader : IAttributeReader {
        public bool ShouldTerminate => false;

        public abstract void Write(M3UFileInfo m3UFileInfo, string value, IEnumerator<string> reader, Uri baseUri);

        public bool Read(M3UFileInfo m3UFileInfo, IEnumerator<string> reader, KeyValuePair<string, string> keyValuePair, Uri baseUri) {
            Write(m3UFileInfo, keyValuePair.Value, reader, baseUri);

            return ShouldTerminate;
        }
    }
}