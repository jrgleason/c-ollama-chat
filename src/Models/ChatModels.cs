using System.Text.Json.Serialization;

namespace ChatApp.Models
{
    public class ChatRequest
    {
        [JsonPropertyName("prompt")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("model")]
        public string Model { get; set; } = "llama2";

        [JsonPropertyName("stream")]
        public bool Stream { get; set; } = false;
    }
    public class ChatResponse
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("response")]
        public string Response { get; set; } = string.Empty;

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("done")]
        public bool Done { get; set; }

        [JsonPropertyName("total_duration")]
        public long TotalDuration { get; set; }        // Custom property for processing time (not from Ollama)
        public long ProcessingTime { get; set; }
    }

    public class ChatStreamResponse
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("response")]
        public string Response { get; set; } = string.Empty;

        [JsonPropertyName("done")]
        public bool Done { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("total_duration")]
        public long TotalDuration { get; set; }
    }

    public class OllamaModelsResponse
    {
        [JsonPropertyName("models")]
        public OllamaModel[]? Models { get; set; }
    }

    public class OllamaModel
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("model")]
        public string? Model { get; set; }

        [JsonPropertyName("size")]
        public long Size { get; set; }

        [JsonPropertyName("digest")]
        public string? Digest { get; set; }

        [JsonPropertyName("modified_at")]
        public DateTime Modified_at { get; set; }
    }
}
