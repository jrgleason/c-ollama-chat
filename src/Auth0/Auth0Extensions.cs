using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace ChatApp.Auth0
{
    public static class Auth0Extensions
    {
        private const string AuthenticationScheme = "Auth0";

        public static AuthenticationBuilder AddAuth0WebAppAuthentication(this AuthenticationBuilder builder, Action<Auth0Options> configureOptions)
        {
            return builder.AddAuth0WebAppAuthentication(AuthenticationScheme, configureOptions);
        }

        public static AuthenticationBuilder AddAuth0WebAppAuthentication(
            this AuthenticationBuilder builder, 
            string authenticationScheme, 
            Action<Auth0Options> configureOptions)
        {
            var auth0Options = new Auth0Options();
            configureOptions(auth0Options);

            return builder.AddOpenIdConnect(authenticationScheme, options =>
            {
                options.Authority = $"https://{auth0Options.Domain}";
                options.ClientId = auth0Options.ClientId;
                options.ClientSecret = auth0Options.ClientSecret;
                options.ResponseType = auth0Options.ResponseType;

                options.Scope.Clear();
                foreach (var scope in auth0Options.Scope)
                {
                    options.Scope.Add(scope);
                }

                options.CallbackPath = auth0Options.CallbackPath;
                options.ClaimsIssuer = auth0Options.ClaimsIssuer;
                options.SaveTokens = auth0Options.SaveTokens;
                options.TokenValidationParameters = auth0Options.TokenValidationParameters;

                // Handle logout redirect
                options.Events = new OpenIdConnectEvents
                {
                    OnRedirectToIdentityProviderForSignOut = (context) =>
                    {
                        var logoutUri = $"https://{auth0Options.Domain}/v2/logout?client_id={auth0Options.ClientId}";

                        var postLogoutUri = context.Properties.RedirectUri;
                        if (!string.IsNullOrEmpty(postLogoutUri))
                        {
                            if (postLogoutUri.StartsWith("/"))
                            {
                                var request = context.Request;
                                postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
                            }
                            logoutUri += $"&returnTo={Uri.EscapeDataString(postLogoutUri)}";
                        }

                        context.Response.Redirect(logoutUri);
                        context.HandleResponse();

                        return Task.CompletedTask;
                    }
                };
            });
        }

        // Extension method for API authentication
        public static AuthenticationBuilder AddAuth0ApiAuthentication(
            this AuthenticationBuilder builder, 
            Action<Auth0ApiOptions> configureOptions)
        {
            var auth0Options = new Auth0ApiOptions();
            configureOptions(auth0Options);

            return builder.AddJwtBearer(options =>
            {
                options.Authority = $"https://{auth0Options.Domain}/";
                options.Audience = auth0Options.Audience;
                
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = $"https://{auth0Options.Domain}/",
                    ValidAudience = auth0Options.Audience
                };
            });
        }
    }

    // Simplified options class just for API authentication
    public class Auth0ApiOptions
    {
        public string Domain { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
    }
}
