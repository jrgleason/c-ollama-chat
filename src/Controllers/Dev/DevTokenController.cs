using ChatApp.Config;
using ChatApp.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers.Dev
{
    [ApiController]
    [Route("dev/token")]
    public class DevTokenController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DevTokenController> _logger;
        private readonly IWebHostEnvironment _environment;

        public DevTokenController(
            IConfiguration configuration,
            ILogger<DevTokenController> logger,
            IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _logger = logger;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult GenerateToken([FromQuery] string userId, [FromQuery] bool isAdmin = false)
        {
            // Only allow in development environment
            if (!_environment.IsDevelopment())
            {
                return NotFound();
            }

            var authConfig = _configuration.GetSection("Auth").Get<AuthConfig>() ?? new AuthConfig();
            
            if (string.IsNullOrEmpty(authConfig.JwtSecret) || 
                string.IsNullOrEmpty(authConfig.Issuer) || 
                string.IsNullOrEmpty(authConfig.Audience))
            {
                return BadRequest("Auth configuration is incomplete. Please check appsettings.json.");
            }

            try
            {
                var token = JwtTokenGenerator.GenerateToken(
                    authConfig.Issuer!,
                    authConfig.Audience!,
                    authConfig.JwtSecret!,
                    userId,
                    isAdmin);

                _logger.LogInformation($"Generated test token for {userId}, admin: {isAdmin}");
                
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating test token");
                return StatusCode(500, new { Error = "Failed to generate token" });
            }
        }
    }
}
