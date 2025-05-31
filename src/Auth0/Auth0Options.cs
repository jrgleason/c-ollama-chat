using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace ChatApp.Auth0
{
    public class Auth0Options : OpenIdConnectOptions
    {
        public string Domain { get; set; } = string.Empty;

        public Auth0Options()
        {
            ResponseType = OpenIdConnectResponseType.Code;
    
            Scope.Clear();
            Scope.Add("openid");
            Scope.Add("profile");
            Scope.Add("email");
    
            CallbackPath = new PathString("/callback");
            ClaimsIssuer = "Auth0";
            SaveTokens = true;
    
            TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = "name"
            };
        }
    }
}
