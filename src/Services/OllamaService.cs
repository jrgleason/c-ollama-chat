using System.Text;
using System.Text.Json;
using ChatApp.Models;

namespace ChatApp.Services
{
    public interface IOllamaService
    {
        Task<ChatResponse> GenerateResponseAsync(string message, string model = "llama2", CancellationToken cancellationToken = default);
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

                var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
                var chatResponse = JsonSerializer.Deserialize<ChatResponse>(responseJson);

                if (chatResponse == null)
                {
                    throw new Exception("Failed to deserialize response from Ollama");
                }

                chatResponse.ProcessingTime = stopwatch.ElapsedMilliseconds;
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
        }
    }
}
