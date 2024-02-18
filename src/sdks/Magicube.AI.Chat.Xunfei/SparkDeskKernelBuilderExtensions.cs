using Magicube.Core;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.TextGeneration;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.DependencyInjection;

namespace Magicube.AI.Chat.Xunfei {
    public static class SparkDeskKernelBuilderExtensions {
        public const string ServiceId = "SparkDesk";
        public static IKernelBuilder WithSparkDeskCompletionService(this IKernelBuilder builder, SparkDeskOption options) {
            var generation = new SparkDeskTextCompletion(options);
            builder.Services.Add(ServiceId, typeof(IChatCompletionService), sp => generation, ServiceLifetime.Singleton);
            builder.Services.Add(ServiceId, typeof(ITextGenerationService), sp => generation, ServiceLifetime.Singleton);
            return builder;
        }
    }
}
