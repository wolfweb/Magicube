using Microsoft.SemanticKernel;

namespace Magicube.AI.Chat.DashScope {
    public static class IKernelBuilderExtension {
        public const string ServiceId = "DashScope";
        public static IKernelBuilder UseDashScope(this IKernelBuilder kernelBuilder, DashScopeOptions options) {
            return kernelBuilder.WithDashScopeCompletionService(options.ApiKey, options.ModelName, serviceId: ServiceId);
        }
    }
}
