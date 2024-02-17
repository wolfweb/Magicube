using Magicube.Core;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Magicube.AI.Faces {
    public static class ServiceCollectionExtension {
        public static IServiceCollection UseDlibFace(this IServiceCollection services, Action<FaceRecognizeOption> config) {
            services.Configure(config);
            services.TryReplace<IFaceRecognizeProvider, FaceRecognizeProvider>();
            return services;
        }
    }
}