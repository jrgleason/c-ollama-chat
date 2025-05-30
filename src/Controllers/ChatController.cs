using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChatApp.Models;
using ChatApp.Services;

namespace ChatApp.Controllers
{
    [ApiController]
    [Route("chat")]
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
                var response = await _ollamaService.GenerateResponseAsync(message.Text, defaultModel, cancellationToken);
                
                return Ok(new { 
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
