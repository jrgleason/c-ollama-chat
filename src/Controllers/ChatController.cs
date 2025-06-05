using ChatApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly ILogger<ChatController> _logger;
        private readonly IOllamaService _ollamaService;
        private readonly IConfiguration _configuration;

        public ChatController(
            ILogger<ChatController> logger,
            IOllamaService ollamaService,
            IConfiguration configuration)
        {
            _logger = logger;
            _ollamaService = ollamaService;
            _configuration = configuration;
        }

        [HttpGet]
        [Authorize] // Requires any authenticated user
        public IActionResult Get()
        {
            return Ok(new { Message = "Welcome to the chat service" });
        }
        [HttpPost("message")]
        [Authorize] // Requires any authenticated user
        public async Task<IActionResult> SendMessage([FromBody] ChatMessage message, CancellationToken cancellationToken)
        {
            var userName = User.Identity?.Name ?? "anonymous";
            _logger.LogInformation($"Message from {userName}: {message.Text}");

            try
            {
                var defaultModel = _configuration["Ollama:DefaultModel"] ?? "llama2";
                var modelToUse = !string.IsNullOrEmpty(message.Model) ? message.Model : defaultModel;
                var response = await _ollamaService.GenerateResponseAsync(message.Text, modelToUse, cancellationToken);

                return Ok(new
                {
                    Response = response.Response,
                    Model = response.Model,
                    ProcessingTimeMs = response.ProcessingTime
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat message");
                return StatusCode(500, new { Error = "An error occurred while processing your request." });
            }
        }
        [HttpPost("stream")]
        [Authorize] // Requires any authenticated user
        public async Task StreamMessage([FromBody] ChatMessage message, CancellationToken cancellationToken)
        {
            var userName = User.Identity?.Name ?? "anonymous";
            _logger.LogInformation($"Streaming message from {userName}: {message.Text}");

            Response.Headers["Content-Type"] = "text/plain; charset=utf-8";
            Response.Headers["Cache-Control"] = "no-cache";
            Response.Headers["Connection"] = "keep-alive";

            try
            {
                var defaultModel = _configuration["Ollama:DefaultModel"] ?? "llama2";
                var modelToUse = !string.IsNullOrEmpty(message.Model) ? message.Model : defaultModel;

                await foreach (var streamResponse in _ollamaService.GenerateStreamResponseAsync(message.Text, modelToUse, cancellationToken))
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    var jsonResponse = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        Response = streamResponse.Response,
                        Model = streamResponse.Model,
                        Done = streamResponse.Done
                    });

                    await Response.WriteAsync($"data: {jsonResponse}\n\n", cancellationToken);
                    await Response.Body.FlushAsync(cancellationToken);

                    if (streamResponse.Done)
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing streaming chat message");
                var errorResponse = System.Text.Json.JsonSerializer.Serialize(new
                {
                    Response = "An error occurred while processing your request.",
                    Model = message.Model ?? "unknown",
                    Done = true,
                    Error = true
                });
                await Response.WriteAsync($"data: {errorResponse}\n\n", cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }
        }

        [HttpGet("models")]
        [Authorize] // Requires any authenticated user
        public IActionResult GetModels()
        {
            // In a real implementation, you would get this from Ollama API
            var models = new[] { "llama2", "mistral", "gemma", "codellama" };
            return Ok(new { Models = models });
        }

        [HttpGet("admin")]
        [Authorize(Policy = "RequireAdminScope")] // Requires admin scope
        public IActionResult AdminAction()
        {
            return Ok(new { Message = "Admin action executed" });
        }
    }

    public class ChatMessage
    {
        public string Text { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
    }
}
