using DashScope;

namespace Magicube.AI.Chat.DashScope {
    public class DashScopeOptions {
        public string ApiKey { get; set; }
        public string ModelName { get; set; } = DashScopeModels.QWenTurbo;
    }
}
