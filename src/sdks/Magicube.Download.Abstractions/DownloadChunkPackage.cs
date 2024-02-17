using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.Download.Abstractions {
    public interface IPackageHandler {
        long Length { get; }
        Task DoneAsync();
        void Write(long position, byte[] bytes, int length);
    }

    public class DownloadChunkPackage : DownloadPackage {
        private readonly IPackageHandler _handler;
        public DownloadChunkPackage(string rawUrl, IPackageHandler packageHandler) : base(rawUrl) {
            _handler = packageHandler;
        }

        public Chunk[] Chunks                   { get; set; }
        public int     ChunkCount               { get; set; } = 1;
        public int     MinSizeOfChunk           { get; set; } = 512;
        public int     RangeStart               { get; set; }
        public bool    RangeDownload            { get; set; } = true;
        public long    TotalFileSize            { get; set; }
        public bool    IsSupportDownloadInRange { get; set; } = true;
        public int     MinimumChunkSize         { get; set; } = 1024 * 1024;
        public int     MaximumChunkSize         { get; set; } = 10 * 1024 * 1024;

        public DownloadChunkPackage CompleteChunk() {
            int recommendedSize = (int)Math.Ceiling((double)TotalFileSize / Environment.ProcessorCount);
            int chunkSize = Math.Clamp(recommendedSize, MinimumChunkSize, MaximumChunkSize);
            ChunkCount = (int)Math.Ceiling(TotalFileSize / (chunkSize * 1f));
            return this;
        }

        public void Validate() {
            foreach (var chunk in Chunks) {
                if (!chunk.IsValidPosition()) {
                    var realLength = _handler?.Length ?? 0;
                    if (realLength <= chunk.Position) {
                        chunk.Clear();
                    }
                }

                if (!IsSupportDownloadInRange)
                    chunk.Clear();
            }
        }

        public long Length => _handler?.Length ?? 0;

        public void Write(long position, byte[] bytes, int length) => _handler.Write(position, bytes, length);

        public Task DoneAsync() => _handler.DoneAsync();
    }

    public class StreamHandler : IPackageHandler, IDisposable {
        private readonly Stream _stream;
        private readonly BlockingCollection<Packet> _inputBag;
        private readonly SemaphoreSlim _queueConsumerLocker = new SemaphoreSlim(0);
        private readonly ManualResetEventSlim _completionEvent = new ManualResetEventSlim(true);
        public StreamHandler(string file, long bufferSize = 4096) : this(bufferSize) {
            _stream              = new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        }

        public StreamHandler(Stream stream, long bufferSize = 4096) : this(bufferSize) { 
            _stream = stream;
        }

        StreamHandler(long bufferSize = 4096) { 
            _inputBag            = new BlockingCollection<Packet>();
            MaxMemoryBufferBytes = bufferSize;
            Task.Run(Watcher).ConfigureAwait(false);        
            _completionEvent.Wait();
        }

        public long MaxMemoryBufferBytes { get; set; }

        public long Length => _stream?.Length ?? 0;

        public void Write(long position, byte[] bytes, int length) {
            _inputBag.Add(new Packet(position, bytes, length));
            _completionEvent.Reset();
            _queueConsumerLocker.Release();
        }

        public async Task DoneAsync() {
            _completionEvent.Wait();
            await _stream.FlushAsync();
            _inputBag.CompleteAdding();
        }

        public void Dispose() {
            _stream.Flush();
            _stream.Dispose();
        }

        private async Task Watcher() {
            while (!_inputBag.IsCompleted) {
                await _queueConsumerLocker.WaitAsync().ConfigureAwait(false);
                var packet = _inputBag.Take();
                await WritePacket(packet).ConfigureAwait(false);
                packet.Dispose();

                if (_inputBag.Count == 0)
                    _completionEvent.Set();
            }
        }

        private async Task WritePacket(Packet packet) {
            if (_stream.CanSeek) {
                _stream.Position = packet.Position;
                Trace.WriteLine($"write stream => {_stream.Position}, package length=>{packet.Length}");
                await _stream.WriteAsync(packet.Data, 0, packet.Length).ConfigureAwait(false);
            }
        }

        sealed class Packet : IDisposable {
            public byte[] Data;
            public long Position;
            public int Length;

            public Packet(long position, byte[] data, int length) {
                Data = data;
                Position = position;
                Length = length;
            }

            public void Dispose() {
                Data = null;
                Position = 0;
                Length = 0;
            }
        }
    }
}