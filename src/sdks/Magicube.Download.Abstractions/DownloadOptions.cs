using Magicube.Core.IO;
using System;
using System.Collections.Generic;

namespace Magicube.Download.Abstractions {
    public class DownloadOptions {
        public bool                       CheckDiskSizeBeforeDownload { get; set; } = true;
                                          
        public int                        MaxTryAgainOnFailover       { get; set; } = 5;

        public long                       MaximumBytesPerSecond       { get; set; } = ThrottledStream.Infinite;        

        public int                        BufferBlockSize             { get; set; } = 4096;
        
        public bool                       ParallelEnable              { get; set; } = true;
                                          
        public int                        ParallelCount               { get; set; } = Environment.ProcessorCount;
                                          
        public string                     StorageFolder               { get; set; }

        public int                        Timeout                     { get; set; } = 1000;

        public Dictionary<string, string> Headers                     { get; set; } = new Dictionary<string, string>();
    }
}