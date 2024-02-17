using Microsoft.SemanticKernel;

namespace Magicube.AI.Chat.DashScope {
    public static class IKernelBuilderExtension {
        public static IKernelBuilder UseDashScope(this IKernelBuilder kernelBuilder, DashScopeOptions options) {
            kernelBuilder.WithDashScopeCompletionService(options.ApiKey, options.ModelName);

            return kernelBuilder;
        }
    }
}
