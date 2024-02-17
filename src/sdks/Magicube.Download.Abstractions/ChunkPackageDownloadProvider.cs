using Magicube.Core.IO;
using Magicube.Core.Tasks;
using Magicube.Net;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.Download.Abstractions {
    public class ChunkPackageDownloadProvider : IPackageDownloadProvider {
        private long MaximumSpeedPerItem;
        private SemaphoreSlim _parallelSemaphore;

        private readonly Curl _curl;
        private readonly ChunkHub _chunkHub;
        private readonly DownloadOptions _options;
        public ChunkPackageDownloadProvider(IOptions<DownloadOptions> options, Curl curl) {
            _curl              = curl;
            _options           = options.Value;
            _chunkHub          = new ChunkHub(options.Value);
            _parallelSemaphore = new SemaphoreSlim(_options.ParallelCount, _options.ParallelCount);
        }

        public const string Key = "ChunkPackage";

        public string Identity => Key;

        public async Task StartDownload(string url, PauseTokenSource pauseTokenSource, CancellationTokenSource cancellationTokenSource) {
            var package = await InitializePackage(url);
            MaximumSpeedPerItem = _options.ParallelEnable ? _options.MaximumBytesPerSecond / Math.Min(package.ChunkCount, package.ParallelCount) : _options.MaximumBytesPerSecond;
            if (_options.ParallelEnable) {
                await ParallelDownload(package, pauseTokenSource.Token, cancellationTokenSource);
            }
            else {
                await SerialDownload(package, pauseTokenSource.Token, cancellationTokenSource);
            }
            await package.DoneAsync();
        }

        private async Task<DownloadChunkPackage> InitializePackage(string url) {
            var packageHandler = new StreamHandler(Path.Combine(_options.StorageFolder, Path.GetFileName(new Uri(url).AbsolutePath)));
            var package = new DownloadChunkPackage(url, packageHandler) {
                ParallelCount = _options.ParallelCount                
            };
            await PackageProbe(url, package);
            _chunkHub.CalcChunks(package);
            return package;
        }

        private async Task PackageProbe(string url, DownloadChunkPackage package) {
            var assistor = new RequestAssistor(url, _curl);
            package.TotalFileSize = await assistor.GetFileSize();
            package.IsSupportDownloadInRange = await assistor.IsSupportDownloadInRange();

            if (package.TotalFileSize <= 1) package.TotalFileSize = 0;
            
            //文件太小或者文件不支持Range下载
            if(package.TotalFileSize < package.MinSizeOfChunk || !package.IsSupportDownloadInRange) {
                //单线程单chunk下载
                package.ChunkCount = 1;
                package.ParallelCount = 1;
                _parallelSemaphore = new SemaphoreSlim(1, 1);
            }
            else {
                package.CompleteChunk();
            }
        }

        private async Task ParallelDownload(DownloadChunkPackage package, PauseToken pauseToken, CancellationTokenSource cancellationTokenSource) {
            var tasks = package.Chunks.Select(x => DownloadAsynInternal(x, package, pauseToken, cancellationTokenSource.Token));
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private async Task SerialDownload(DownloadChunkPackage package, PauseToken pauseToken, CancellationTokenSource cancellationTokenSource) {
            foreach (var mediaFile in package.Chunks) {
                await DownloadAsynInternal(mediaFile, package, pauseToken, cancellationTokenSource.Token).ConfigureAwait(false);
            }
        }

        private async Task DownloadAsynInternal(Chunk chunk, DownloadChunkPackage package, PauseToken pauseToken, CancellationToken cancelToken) {
            try {
                await _parallelSemaphore.WaitAsync();
                await _curl.Get(package.RawUrl, request => {
                    var startOffset = chunk.Start + chunk.Position;
                    // has limited range
                    if (chunk.End > 0 && (package.ChunkCount > 1 || chunk.Position > 0 || package.RangeDownload)) {
                        if (startOffset < chunk.End)
                            request.Headers.Range = new RangeHeaderValue(startOffset, chunk.End);
                        else
                            request.Headers.Range = new RangeHeaderValue(startOffset, null);
                    }
                }).ReadAsStream(async x => {
                    int readSize = 1;
                    CancellationToken? innerToken = null;
                    using (var stream = new ThrottledStream(x, MaximumSpeedPerItem)) {
                        using (cancelToken.Register(stream.Close)) {
                            try {
                                while (readSize > 0) {
                                cancelToken.ThrowIfCancellationRequested();
                                await pauseToken.PauseIfRequestedAsync().ConfigureAwait(false);

                                byte[] buffer = new byte[_options.BufferBlockSize];

                                using var innerCts = CancellationTokenSource.CreateLinkedTokenSource(cancelToken);
                                innerToken = innerCts.Token;
                                innerCts.CancelAfter(chunk.Timeout);

                                readSize = await stream.ReadAsync(buffer, 0, buffer.Length, innerToken.Value).ConfigureAwait(false);

                                if (readSize > 0) {
                                    package.Write(chunk.Start + chunk.Position - package.RangeStart, buffer, readSize);
                                    chunk.Position += readSize;
                                }
                            }
                            }
                            catch (Exception e) {
                                Console.WriteLine(e);
                            }
                        }
                    }

                    return x;
                });
            }
            finally {
                _parallelSemaphore.Release();
            }
        }
    }
}
