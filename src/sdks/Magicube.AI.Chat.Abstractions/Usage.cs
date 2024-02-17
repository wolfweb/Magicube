using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Magicube.AI.Chat.Abstractions {
    public record Usage {
        [JsonProperty("completion_tokens")]
        public int CompletionTokens { get; set; }

        [JsonProperty("prompt_tokens")]
        public int PromptTokens     { get; set; }

        [JsonProperty("total_tokens")]
        public int TotalTokens      { get; set; }
    }

    public record ChatParameters {
        [JsonProperty("result_format")]
        public string ResultFormat { get; set; }

        [JsonProperty("seed")]
        public ulong Seed { get; set; }

        [JsonProperty("max_tokens")]
        public int? MaxTokens { get; set; }

        [JsonProperty("top_p")]
        public float? TopP { get; set; }

        [JsonProperty("top_k")]
        public int? TopK { get; set; }

        [JsonProperty("repetition_penalty")]
        public float? RepetitionPenalty { get; set; }

        [JsonProperty("temperature")]
        public float? Temperature { get; set; }

        [JsonProperty("stop")]
        public object? Stop { get; set; }

        [JsonProperty("enable_search")]
        public bool? EnableSearch { get; set; }

        [JsonProperty("incremental_output")]
        public bool? IncrementalOutput { get; set; }

        [JsonProperty("stream")]
        public bool Stream { get; set; }

        [JsonProperty("do_sample")]
        public bool DoSample { get; set; }

        [JsonProperty("penalty_score")]
        public float? PenaltyScore { get; set; }

        [JsonProperty("system")]
        public string System { get; set; }

        [JsonProperty("disable_search")]
        public bool DisableSearch { get; set; }

        [JsonProperty("enable_citation")]
        public bool EnableCitation { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("presence_penalty")]
        public float? PresencePenalty { get; set; }

        [JsonProperty("frequency_penalty")]
        public float? FrequencyPenalty { get; set; }

        [JsonProperty("max_output_tokens")]
        public int? MaxOutputTokens { get; set; }
    }
}