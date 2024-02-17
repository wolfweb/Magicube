using System;

namespace Magicube.Download.Abstractions {
    public class Chunk {
        public string Id                    { get; set; }                                          
        public long   End                   { get; set; }
        public long   Start                 { get; set; }
        public long   Position              { get; set; }
                      
        public int    Timeout               { get; set; }
        public int    FailoverCount         { get; private set; }
        public int    MaxTryAgainOnFailover { get; set; }
        public long   Length                => End - Start + 1;

        public Chunk() {
            Id = Guid.NewGuid().ToString("n");
        }

        public Chunk(long start, long end) : this() {
            Start = start;
            End   = end;
        }

        public bool CanTryAgainOnFailover() {
            return FailoverCount++ < MaxTryAgainOnFailover;
        }

        public void Clear() {
            Position      = 0;
            FailoverCount = 0;
        }

        public bool IsDownloadCompleted() {
            var isNoneEmptyFile          = Length > 0;
            var isChunkedFilledWithBytes = Start + Position >= End;
            return isNoneEmptyFile && isChunkedFilledWithBytes;
        }

        public bool IsValidPosition() {
            return Length == 0 || (Position >= 0 && Position <= Length);
        }
    }
}