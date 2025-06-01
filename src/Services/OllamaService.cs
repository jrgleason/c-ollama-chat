using System.Text;
using System.Text.Json;
using ChatApp.Models;

namespace ChatApp.Services
{    public interface IOllamaService
    {
        Task<ChatResponse> GenerateResponseAsync(string message, string model = "llama2", CancellationToken cancellationToken = default);
        Task<string[]> GetAvailableModelsAsync(CancellationToken cancellationToken = default);
    }

    public class OllamaService : IOllamaService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OllamaService> _logger;

        public OllamaService(HttpClient httpClient, IConfiguration configuration, ILogger<OllamaService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;

            // Configure HttpClient with the Ollama API base URL
            _httpClient.BaseAddress = new Uri(_configuration["Ollama:BaseUrl"] ?? "http://localhost:11434");
        }

        public async Task<ChatResponse> GenerateResponseAsync(string message, string model = "llama2", CancellationToken cancellationToken = default)
        {
            try
            {
                var request = new ChatRequest
                {
                    Message = message,
                    Model = model,
                    Stream = false
                };

                var jsonContent = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                
                var response = await _httpClient.PostAsync("/api/generate", content, cancellationToken);
                response.EnsureSuccessStatusCode();

                stopwatch.Stop();

                var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);                var chatResponse = JsonSerializer.Deserialize<ChatResponse>(responseJson);

                if (chatResponse == null)
                {
                    throw new Exception("Failed to deserialize response from Ollama");
                }

                chatResponse.ProcessingTime = stopwatch.ElapsedMilliseconds;
                
                _logger.LogInformation($"Ollama response for model {model}: {chatResponse.Response.Substring(0, Math.Min(50, chatResponse.Response.Length))}...");
                
                return chatResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Ollama API");
                return new ChatResponse
                {
                    Model = model,
                    Response = "Sorry, there was an error processing your message.",
                    ProcessingTime = 0
                };
            }
        }        public async Task<string[]> GetAvailableModelsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Fetching models from Ollama API at {BaseUrl}", _httpClient.BaseAddress);
                
                var response = await _httpClient.GetAsync("/api/tags", cancellationToken);
                response.EnsureSuccessStatusCode();

                var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogInformation("Received response from Ollama: {Response}", responseJson.Substring(0, Math.Min(200, responseJson.Length)));
                
                var modelsResponse = JsonSerializer.Deserialize<OllamaModelsResponse>(responseJson);

                if (modelsResponse?.Models == null)
                {
                    _logger.LogWarning("No models returned from Ollama API");
                    return new[] { "llama2" }; // Default fallback
                }

                var modelNames = modelsResponse.Models.Select(m => m.Name ?? "unknown").ToArray();
                _logger.LogInformation("Found {Count} models: {Models}", modelNames.Length, string.Join(", ", modelNames));
                
                return modelNames;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching models from Ollama API");
                // Return default models as fallback
                return new[] { "llama2", "llama3", "mistral", "gemma", "codellama" };
            }
        }
    }
}
