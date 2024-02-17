using Magicube.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Magicube.AI.Faces {
    public static class ServiceCollectionExtension {
		public static IServiceCollection UseSeetaFace(this IServiceCollection services) {
			services.TryReplace<IFaceRecognizeProvider, FaceRecognizeProvider>();
			return services;
		}
	}
}