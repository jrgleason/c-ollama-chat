using ChatApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers
{
    [ApiController]
    [Route("api/config")]
    public class ConfigController : ControllerBase
    {
        private readonly ILogger<ConfigController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IOllamaService _ollamaService;

        public ConfigController(ILogger<ConfigController> logger, IConfiguration configuration, IOllamaService ollamaService)
        {
            _logger = logger;
            _configuration = configuration;
            _ollamaService = ollamaService;
        }

        [HttpGet("user")]
        [Authorize]
        public IActionResult GetUserConfig()
        {
            // In a real implementation, you would retrieve user-specific settings
            // from a database based on the authenticated user
            var userId = User.Identity?.Name;

            return Ok(new
            {
                UserId = userId,
                DefaultModel = _configuration["Ollama:DefaultModel"] ?? "llama2",
                Theme = "light", // Example user preference
                HistoryEnabled = true
            });
        }
        [HttpPost("user")]
        [Authorize]
        public IActionResult UpdateUserConfig([FromBody] UserConfig config)
        {
            // In a real implementation, you would save these settings to a database
            _logger.LogInformation($"Updated config for user {User.Identity?.Name}");

            return Ok(new { Message = "Configuration updated successfully" });
        }

        [HttpGet("models")]
        public async Task<IActionResult> GetAvailableModels()
        {
            try
            {
                var models = await _ollamaService.GetAvailableModelsAsync();
                return Ok(models);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching models from Ollama");
                // Return fallback models
                var fallbackModels = new[] { "llama2", "llama3", "mistral", "gemma", "codellama" };
                return Ok(fallbackModels);
            }
        }
        [HttpGet("auth")]
        public IActionResult GetAuthConfig()
        {
            // Return Auth0 configuration for the client
            var authConfig = new
            {
                Domain = _configuration["Auth0:Domain"] ?? "",
                ClientId = _configuration["Auth0:ClientId"] ?? "",
                Audience = _configuration["Auth0:Audience"] ?? "",
                Scope = _configuration["Auth0:Scope"] ?? "openid profile email"
            };

            return Ok(authConfig);
        }
    }

    public class UserConfig
    {
        public string DefaultModel { get; set; } = string.Empty;
        public string Theme { get; set; } = string.Empty;
        public bool HistoryEnabled { get; set; }
    }
}
