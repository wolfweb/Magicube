using Magicube.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Magicube.Download.Abstractions {
    public static class ServiceCollectionExtension {
        public static IServiceCollection AddDownload(this IServiceCollection services, Action<DownloadOptions> handler) {
            services.TryAddTransient<IDownloadService, DownloadService>();
            services.TryAddTransient<IPackageDownloadProvider, ChunkPackageDownloadProvider>(ChunkPackageDownloadProvider.Key);
            services.Configure<DownloadOptions>(x => handler?.Invoke(x));

            return services;
        }
    }
}