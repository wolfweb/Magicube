using Microsoft.Extensions.DependencyInjection;
using Magicube.Core;
using System;
using Magicube.Download.Abstractions;
using Magicube.Medias.Hls;
using Xunit;
using System.Threading.Tasks;
using Magicube.Net;
using System.IO;
using static Magicube.Media.Test.DownloadServiceTest;

namespace Magicube.Media.Test {
    public class DownloadServiceTest {
        private readonly IServiceProvider ServiceProvider;

        public DownloadServiceTest() {
            ServiceProvider = new ServiceCollection()
                .AddCore()
                .AddHttpServices()
                .Configure<DownloadOptions>(x => {
                    x.StorageFolder = @"E:\temp";
                })
                .AddTransient<IPackageDownloadProvider, ChunkPackageDownloadProvider>("default")
                .AddTransient<IPackageDownloadProvider, M3UPackageDownloadProvider>("m3u")
                .AddTransient<IDownloadService, DownloadService>()
                .BuildServiceProvider();
        }

        [Theory]
        [InlineData("https://349-26.vod.tv.itc.cn/sohu/v1/Tms7TmwC0EI7TJNMN6XBTKbbqMAi86CUyAXUz8465m47fFoGRM1O0r.mp4")]
        public async Task Package_Chunk_Download_Test(string url) {
            var service = ServiceProvider.GetService<IDownloadService>();
            var downloadProvider = ServiceProvider.GetService<IPackageDownloadProvider>("default");
            await service.StartDownloadAsync(url, downloadProvider);
        }

        [Fact]
        public async Task ChunkPackage_Test() {
            using var mem = new MemoryStream();
            var packageHandler = new StreamHandler(mem);
            var package = new DownloadChunkPackage(string.Empty, packageHandler);
            var size = 1024 * 64;
            package.TotalFileSize = size;
            package.CompleteChunk();
            var datas = DummyData.GenerateRandomBytes(size);
            new ChunkHub(new DownloadOptions()).CalcChunks(package);
            for (int i = 0; i < package.Chunks.Length; i++) {
                var chunk = package.Chunks[i];
                package.Write(chunk.Start, datas, datas.Length);
            }
            await package.DoneAsync();
            Assert.Equal(mem.Length, package.TotalFileSize);
        }

        [Theory]
        [InlineData(@"E:\Sources\CSharp\N_m3u8DL-CLI\N_m3u8DL-CLI\bin\Debug\m3u8.txt")]
        public async Task Package_M3U_Download_Test(string url) {            
            var service = ServiceProvider.GetService<IDownloadService>();
            var downloadProvider = ServiceProvider.GetService<IPackageDownloadProvider>("m3u");
            await service.StartDownloadAsync(url, downloadProvider);
        }

        public static class DummyData {
            public static byte[] GenerateRandomBytes(int length) {
                if (length < 1)
                    throw new ArgumentException("length has to be > 0");

                Random rand = new Random();
                byte[] buffer = new byte[length];
                rand.NextBytes(buffer);
                return buffer;
            }

            public static byte[] GenerateOrderedBytes(int length) {
                if (length < 1)
                    throw new ArgumentException("length has to be > 0");

                byte[] buffer = new byte[length];
                for (int i = 0; i < length; i++) {
                    buffer[i] = (byte)(i % 256);
                }

                return buffer;
            }
        }
    }
}
