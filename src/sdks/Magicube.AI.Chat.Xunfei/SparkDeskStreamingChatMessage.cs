using System.Collections.Generic;
using Microsoft.SemanticKernel.ChatCompletion;
using Sdcb.SparkDesk;
using Microsoft.SemanticKernel;

namespace Magicube.AI.Chat.Xunfei {
    public class SparkDeskStreamingChatMessage : StreamingChatMessageContent {
        public SparkDeskStreamingChatMessage(StreamedChatResponse response, IReadOnlyDictionary<string, object> metadata = null)
            : base(AuthorRole.Assistant, response.Text, response, metadata: metadata) { }
    }
}
