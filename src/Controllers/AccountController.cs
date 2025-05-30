using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IConfiguration configuration, ILogger<AccountController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("Login")]
        public async Task<IActionResult> Login(string returnUrl = "/")
        {
            // Trigger Auth0 authentication flow
            var authProperties = new AuthenticationProperties
            {
                RedirectUri = returnUrl
            };

            return Challenge(authProperties, "Auth0");
        }

        [HttpGet("Logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // Sign out of the cookie authentication
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Sign out of Auth0 
            return SignOut(
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("Index", "Home") ?? "/"
                },
                "Auth0");
        }

        [HttpGet("Profile")]
        [Authorize]
        public IActionResult Profile()
        {
            return Json(User.Claims.Select(c => new { Type = c.Type, Value = c.Value }));
        }
    }
}
