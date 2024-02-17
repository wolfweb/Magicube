using Magicube.Core;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.TextGeneration;
using Sdcb.SparkDesk;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Linq;

namespace Magicube.AI.Chat.Xunfei {
    public class SparkDeskOption {
        public ModelVersion Version   { get; set; }
        public string       ApiId     { get; set; }
        public string       ApiKey    { get; set; }
        public string       ApiSecret { get; set; }
    }

    public class SparkDeskTextCompletion : IChatCompletionService, ITextGenerationService {
        private readonly SparkDeskClient _client;
        private readonly SparkDeskOption _sparkDeskOption;
        private readonly Dictionary<string, object> _attributes = new();

        public SparkDeskTextCompletion(SparkDeskOption options) {
            _sparkDeskOption = options;
            _client = new SparkDeskClient(options.ApiId, options.ApiKey, options.ApiSecret);
        }

        public IReadOnlyDictionary<string, object> Attributes => _attributes;

        public async Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default) {
            var settings = OpenAIPromptExecutionSettings.FromExecutionSettings(executionSettings);
            var response = await _client.ChatAsync(_sparkDeskOption.Version, ChatHistoryToMessages(chatHistory), cancellationToken: cancellationToken);

            var metadata = GetResponseMetadata(response);

            return new List<ChatMessageContent>() { new SparkDeskChatMessage(response, metadata) }.AsReadOnly();
        }

        public async IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings executionSettings = null, Kernel kernel = null, [EnumeratorCancellation] CancellationToken cancellationToken = default) {
            var settings = OpenAIPromptExecutionSettings.FromExecutionSettings(executionSettings);

            var responses = _client.ChatAsStreamAsync(_sparkDeskOption.Version, ChatHistoryToMessages(chatHistory), cancellationToken: cancellationToken);
            await foreach (var response in responses) {
                var metadata = GetResponseMetadata(response);
                yield return new SparkDeskStreamingChatMessage(response, metadata);
            }
        }

        public async IAsyncEnumerable<StreamingTextContent> GetStreamingTextContentsAsync(string prompt, PromptExecutionSettings executionSettings = null, Kernel kernel = null, [EnumeratorCancellation] CancellationToken cancellationToken = default) {
            var settings = OpenAIPromptExecutionSettings.FromExecutionSettings(executionSettings);

            var responses = _client.ChatAsStreamAsync(_sparkDeskOption.Version, new[] { ChatMessage.FromUser(prompt) }, cancellationToken: cancellationToken);
            await foreach (var response in responses) {
                var metadata = GetResponseMetadata(response);
                yield return new StreamingTextContent(response.Text, metadata: metadata);
            }
        }

        public async Task<IReadOnlyList<TextContent>> GetTextContentsAsync(string prompt, PromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default) {
            var settings = OpenAIPromptExecutionSettings.FromExecutionSettings(executionSettings);

            var response = await _client.ChatAsync(_sparkDeskOption.Version, new[] { ChatMessage.FromUser(prompt) }, cancellationToken: cancellationToken);

            var metadata = GetResponseMetadata(response);

            return new List<TextContent>() { new(response.Text, metadata: metadata) }.AsReadOnly();
        }
        
        private static IReadOnlyDictionary<string, object> GetResponseMetadata(StreamedChatResponse response) {
            return new Dictionary<string, object>() {
                [nameof(response.Usage)] = response.Usage,
                [nameof(response.Text)] = response.Text
            };
        }

        private static IReadOnlyDictionary<string, object> GetResponseMetadata(ChatResponse response) {
            return new Dictionary<string, object>() {
                [nameof(response.Usage)] = response.Usage,
                [nameof(response.Text)] = response.Text
            };
        }

        private ChatMessage[] ChatHistoryToMessages(ChatHistory chatHistory) {
            if (chatHistory.Count == 1) {
                return new[] { ChatMessage.FromUser(chatHistory[0].Content) };
            }

            return chatHistory.Select(m => new ChatMessage(AuthorRoleToMessageRole(m.Role), m.Content)).ToArray();
        }

        private string AuthorRoleToMessageRole(AuthorRole role) {
            if (AuthorRoleToMessageRoleMap.TryGetValue(role, out var messageRole)) {
                return messageRole;
            }
            return AuthorRole.User.Label;
        }

        private static readonly Dictionary<AuthorRole, string> AuthorRoleToMessageRoleMap = new Dictionary<AuthorRole, string>
        {
            { AuthorRole.User, AuthorRole.User.Label },
            { AuthorRole.Assistant, AuthorRole.Assistant.Label},
            { AuthorRole.System, AuthorRole.System.Label}
        };
    }
}
