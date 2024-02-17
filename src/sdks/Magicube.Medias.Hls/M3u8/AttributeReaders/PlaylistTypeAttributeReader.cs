using System;
using System.Collections.Generic;

namespace Magicube.Medias.Hls {
    [M3U8Reader("#EXT-X-PLAYLIST-TYPE", typeof(PlaylistTypeAttributeReader))]
    internal class PlaylistTypeAttributeReader : IAttributeReader {
        public bool ShouldTerminate => false;

        public void Write(M3UFileInfo fileInfo, string value, IEnumerator<string> reader, Uri baseUri) {
            fileInfo.PlaylistType = value;
        }
    }
}