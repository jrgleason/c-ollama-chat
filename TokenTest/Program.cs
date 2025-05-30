using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace TokenTest
{
    // Copy of the JwtTokenGenerator class
    public static class JwtTokenGenerator
    {
        public static string GenerateToken(
            string issuer, 
            string audience, 
            string secretKey, 
            string userId,
            bool isAdmin = false,
            int expiryMinutes = 60)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            if (isAdmin)
            {
                claims.Add(new Claim("scope", "site:admin"));
            }

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class Program
    {
        static void Main()
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

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
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
