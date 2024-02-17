using Magicube.Download.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;
using Magicube.Net;
using Microsoft.Extensions.Options;
using Magicube.Core.Tasks;
using System.Net.Http;
using Magicube.Core.Encrypts;
using Magicube.Core;

namespace Magicube.Medias.Hls {
    public class M3UPackageDownloadProvider : IPackageDownloadProvider {
        private readonly Curl _curl;
        private readonly DownloadOptions _options;
        private readonly SemaphoreSlim _parallelSemaphore;
        private readonly Md5CryptoEncryptProvider _cryptoEncryptProvider;
        private readonly M3UFileReaderWithStream _m3UFileReaderWithStream;

        public M3UPackageDownloadProvider(IOptions<DownloadOptions> options,Curl curl) {
            _curl                    = curl;
            _options                 = options.Value;
            _cryptoEncryptProvider   = new Md5CryptoEncryptProvider();
            _parallelSemaphore       = new SemaphoreSlim(_options.ParallelCount, _options.ParallelCount);
            _m3UFileReaderWithStream = new M3UFileReaderWithStream(AttributeReaderRoot.Instance.AttributeReaders);
        }

        public const string Key = "M3UPackage";

        public string Identity => Key;

        public async Task StartDownload(string url, PauseTokenSource pauseTokenSource, CancellationTokenSource cancellationTokenSource) {
            var package = await InitializePackage(url);
            if (_options.ParallelEnable) {
                await ParallelDownload(package, pauseTokenSource.Token, cancellationTokenSource);
            }
            else {
                await SerialDownload(package, pauseTokenSource.Token, cancellationTokenSource);
            }
        }

        private async Task ParallelDownload(DownloadM3u8Package package, PauseToken pauseToken, CancellationTokenSource cancellationTokenSource) {
            var tasks = package.M3UFile.MediaFiles.Select(x => DownloadAsynInternal(x, package, pauseToken, cancellationTokenSource.Token));
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private async Task SerialDownload(DownloadM3u8Package package, PauseToken pauseToken, CancellationTokenSource cancellationTokenSource) {
            foreach (var mediaFile in package.M3UFile.MediaFiles) {
                await DownloadAsynInternal(mediaFile, package, pauseToken, cancellationTokenSource.Token).ConfigureAwait(false);
            }
        }

        private async Task DownloadAsynInternal(M3UMediaInfo mediaFile, DownloadM3u8Package package, PauseToken pauseToken, CancellationToken cancelToken) {
            try {
                await _parallelSemaphore.WaitAsync();
                var stream = await _curl.Get(mediaFile.Uri).ReadAsStream(x => {
                    if (package.M3UFile.Key != null) {
                        return x.AesDecrypt(package.M3UFile.Key.BKey, package.M3UFile.Key.IV);
                    }
                    return x;
                });
                var file = Path.Combine(package.M3UFolder, mediaFile.Title);
                using FileStream fileobject = File.Create(file);
                await stream.CopyToAsync(fileobject);
                await pauseToken.PauseIfRequestedAsync().ConfigureAwait(false);
            }
            finally {
                _parallelSemaphore.Release();
            }
        }

        protected virtual async Task<DownloadM3u8Package> InitializePackage(string url) {
            var package     = new DownloadM3u8Package(url) { 
                FileName = Path.Combine(_options.StorageFolder, Path.GetFileName(new Uri(url).AbsolutePath))
            };

            var uri = new Uri(url);
            if (uri.IsFile) {
                package.M3UFile = _m3UFileReaderWithStream.GetM3u8FileInfo(uri);
            }
            else {
                package.M3UFile = await GetM3u8FileInfo(url, _options.Headers);
            }

            var hash = _cryptoEncryptProvider.Encrypt(url);

            if (package.M3UFile.Key != null) {
                await Initialization(package.M3UFile);
            }

            package.StorageFolder = Path.Combine(_options.StorageFolder, hash);

            if(!Directory.Exists(package.StorageFolder)) {
                Directory.CreateDirectory(package.StorageFolder);
            }

            if (!Directory.Exists(package.M3UFolder)) {
                Directory.CreateDirectory(package.M3UFolder);
            }

            return package;
        }

        protected virtual async ValueTask Initialization(M3UFileInfo m3UFileInfo, CancellationToken cancellationToken = default) {
            if (m3UFileInfo.Key is null)
                throw new InvalidDataException("没有可用的密钥信息");

            if (m3UFileInfo.Key.Uri != null && m3UFileInfo.Key.BKey == null) {
                try {
                    byte[] data = m3UFileInfo.Key.Uri.IsFile
                        ? await File.ReadAllBytesAsync(m3UFileInfo.Key.Uri.OriginalString, cancellationToken)
                        : await _curl.Get(m3UFileInfo.Key.Uri, client => {
                            foreach (var header in _options.Headers) {
                                client.Headers.TryAddWithoutValidation(header.Key, header.Value);
                            }
                        }).ReadAsBytes();

                    m3UFileInfo.Key.BKey = data.TryParseKey(m3UFileInfo.Key.Method);
                }
                catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested) {
                    throw new HttpRequestException("密钥获取失败");
                }
                catch (HttpRequestException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound) {
                    throw new HttpRequestException("获取密钥失败，没有找到任何数据", e.InnerException, e.StatusCode);
                }
            }
            else {
                m3UFileInfo.Key.BKey = m3UFileInfo.Key.BKey != null
                    ? m3UFileInfo.Key.BKey.TryParseKey(m3UFileInfo.Key.Method)
                    : throw new InvalidDataException("密钥为空");
            }
        }

        protected virtual async Task<(Uri, Stream)> GetM3u8FileStreamAsync(string url, IEnumerable<KeyValuePair<string, string>> headers, CancellationToken cancellationToken = default) {
            var ctx = _curl.Get(url, client => {
                foreach (var header in headers) {
                    client.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            });

            return (ctx.Response.RequestMessage.RequestUri, await ctx.Response.Content.ReadAsStreamAsync());
        }

        protected async Task<M3UFileInfo> GetM3u8FileInfo(string url, IEnumerable<KeyValuePair<string, string>> headers, CancellationToken cancellationToken = default) {
            (Uri requestUrl, Stream stream) = await GetM3u8FileStreamAsync(url, headers, cancellationToken);
            M3UFileInfo m3uFileInfo = _m3UFileReaderWithStream.GetM3u8FileInfo(requestUrl ?? new Uri(url), stream);
            if (m3uFileInfo.Streams != null && m3uFileInfo.Streams.Any()) {
                M3UStreamInfo m3UStreamInfo = m3uFileInfo.Streams.Count > 1 ? m3uFileInfo.Streams.OrderByDescending(s => s.Bandwidth).First() : m3uFileInfo.Streams.First();
                return await GetM3u8FileInfo(m3UStreamInfo.Uri.ToString(), headers, cancellationToken);
            }
            return m3uFileInfo;
        }
    }
}