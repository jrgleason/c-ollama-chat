using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ChatApp.Config
{
    public static class SecurityConfig
    {
        public static void AddSecurityServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Get OAuth settings from configuration
            var authConfig = configuration.GetSection("Auth").Get<AuthConfig>() ?? new AuthConfig();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // For development environment, you might want to disable certificate validation
                options.RequireHttpsMetadata = false;

                // If using JWT with a secret
                if (!string.IsNullOrEmpty(authConfig.JwtSecret))
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = !string.IsNullOrEmpty(authConfig.Issuer),
                        ValidateAudience = !string.IsNullOrEmpty(authConfig.Audience),
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = authConfig.Issuer,
                        ValidAudience = authConfig.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(authConfig.JwtSecret ?? "defaultsecretkey"))
                    };
                }
                // If using OAuth with an issuer URI
                else if (!string.IsNullOrEmpty(authConfig.Issuer))
                {
                    options.Authority = authConfig.Issuer;
                    options.Audience = authConfig.Audience;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = authConfig.Issuer,
                        ValidAudience = authConfig.Audience
                    };
                }
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAuthenticatedUser", policy =>
                    policy.RequireAuthenticatedUser());

                options.AddPolicy("RequireAdminScope", policy =>
                    policy.RequireAuthenticatedUser()
                        .RequireClaim("scope", "site:admin"));

                // Default policy
                options.DefaultPolicy = options.GetPolicy("RequireAuthenticatedUser") ?? 
                    new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
            });
        }

        public static void UseSecurityConfig(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}
