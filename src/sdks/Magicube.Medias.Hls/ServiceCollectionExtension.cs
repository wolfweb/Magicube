using Magicube.Download.Abstractions;
using System;
using Microsoft.Extensions.DependencyInjection;
using Magicube.Core;
using Magicube.Media.Hls.FileTypes;

namespace Magicube.Medias.Hls {
    public static class ServiceCollectionExtension {
        public static IServiceCollection AddM3UDownload(this IServiceCollection services, Action<DownloadOptions> handler) {
            services.AddDownload(handler);
            services.TryAddTransient<IPackageDownloadProvider, M3UPackageDownloadProvider>(M3UPackageDownloadProvider.Key);

            FileTypeChecker.FileTypeValidator.RegisterCustomTypes(typeof(M3uFile).Assembly);

            return services;
        }
    }
}