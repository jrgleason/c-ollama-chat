using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace ChatApp.Config
{
    public class AuthConfig
    {
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public string? JwtSecret { get; set; }
    }
}
