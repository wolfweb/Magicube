using System;

namespace Magicube.Medias.Hls {
    public class M3UStreamInfo {
        public string Resolution { get; set; } = default!;
        public int?   ProgramId  { get; set; }
        public int?   Bandwidth  { get; set; }
        public string Codecs     { get; set; } = default!;
        public Uri    Uri        { get; set; } = default!;
    }
}