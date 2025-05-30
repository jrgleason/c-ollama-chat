using System;
using ChatApp.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace ChatApp.Test
{
    public class Program
    {
        public static void Main()
        {
            // Test parameters
            string issuer = "https://dev-auth-server.com/";
            string audience = "dev-api-identifier";
            string secretKey = "dev-secret-key-at-least-32-chars-long";
            string userId = "test-user";
            
            // Generate a regular user token
            Console.WriteLine("Generating regular user token...");
            string userToken = JwtTokenGenerator.GenerateToken(issuer, audience, secretKey, userId, false);
            Console.WriteLine($"User Token: {userToken}");
            
            // Generate an admin token
            Console.WriteLine("\nGenerating admin token...");
            string adminToken = JwtTokenGenerator.GenerateToken(issuer, audience, secretKey, userId, true);
            Console.WriteLine($"Admin Token: {adminToken}");
            
            // Decode and validate the tokens to show their contents
            Console.WriteLine("\nDecoding tokens to verify contents:");
            DecodeToken(userToken, "Regular User Token");
            DecodeToken(adminToken, "Admin Token");
        }
        
        private static void DecodeToken(string token, string tokenType)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                
                Console.WriteLine($"\n{tokenType} Details:");
                Console.WriteLine($"Issuer: {jwtToken.Issuer}");
                Console.WriteLine($"Audience: {jwtToken.Audiences.FirstOrDefault()}");
                Console.WriteLine($"Expiration: {jwtToken.ValidTo}");
                
                Console.WriteLine("Claims:");
                foreach (var claim in jwtToken.Claims)
                {
                    Console.WriteLine($"  {claim.Type}: {claim.Value}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error decoding token: {ex.Message}");
            }
        }
    }
}
