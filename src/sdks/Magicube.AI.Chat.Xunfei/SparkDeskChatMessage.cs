using System.Collections.Generic;
using Microsoft.SemanticKernel.ChatCompletion;
using Sdcb.SparkDesk;
using Microsoft.SemanticKernel;

namespace Magicube.AI.Chat.Xunfei {
    public class SparkDeskChatMessage : ChatMessageContent {
        public SparkDeskChatMessage(ChatResponse response, IReadOnlyDictionary<string, object> metadata = null)
            : base(AuthorRole.Assistant, response.Text, metadata: metadata) { }
    }
}
