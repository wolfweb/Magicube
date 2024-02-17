using System;
using System.Collections.Generic;

namespace Magicube.Medias.Hls {
    public interface IAttributeReader {
        bool ShouldTerminate { get; }
        void Write(M3UFileInfo m3UFileInfo, string value, IEnumerator<string> reader, Uri baseUri);
    }
}