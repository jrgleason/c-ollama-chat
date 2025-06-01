using ChatApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Configure environment variable mapping for Auth0
// ASP.NET Core will automatically map AUTH0_DOMAIN to Auth0:Domain, etc.
builder.Configuration.AddEnvironmentVariables(prefix: "AUTH0_");

// Alternative explicit mapping if needed
var auth0Config = new List<KeyValuePair<string, string?>>();
if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AUTH0_DOMAIN")))
    auth0Config.Add(new("Auth0:Domain", Environment.GetEnvironmentVariable("AUTH0_DOMAIN")));
if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AUTH0_CLIENT_ID")))
    auth0Config.Add(new("Auth0:ClientId", Environment.GetEnvironmentVariable("AUTH0_CLIENT_ID")));
if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AUTH0_AUDIENCE")))
    auth0Config.Add(new("Auth0:Audience", Environment.GetEnvironmentVariable("AUTH0_AUDIENCE")));
if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AUTH0_SCOPE")))
    auth0Config.Add(new("Auth0:Scope", Environment.GetEnvironmentVariable("AUTH0_SCOPE")));

if (auth0Config.Any())
    builder.Configuration.AddInMemoryCollection(auth0Config);

// Add debugging to show Auth0 configuration values
Console.WriteLine("=== AUTH0 CONFIGURATION DEBUG ===");
var auth0Vars = new[] { "AUTH0_DOMAIN", "AUTH0_CLIENT_ID", "AUTH0_AUDIENCE", "AUTH0_SCOPE" };
foreach (var varName in auth0Vars)
{
    var envValue = Environment.GetEnvironmentVariable(varName);
    
    // Use the same key mapping as our manual configuration
    var configKey = varName switch
    {
        "AUTH0_DOMAIN" => "Auth0:Domain",
        "AUTH0_CLIENT_ID" => "Auth0:ClientId", 
        "AUTH0_AUDIENCE" => "Auth0:Audience",
        "AUTH0_SCOPE" => "Auth0:Scope",
        _ => $"Auth0:{varName.Replace("AUTH0_", "")}"
    };
    
    var configValue = builder.Configuration[configKey];
    var displayEnv = string.IsNullOrEmpty(envValue) ? "(not set)" : 
                     envValue.Length > 8 ? envValue.Substring(0, 8) + "..." : envValue;
    var displayConfig = string.IsNullOrEmpty(configValue) ? "(not set)" : 
                        configValue.Length > 8 ? configValue.Substring(0, 8) + "..." : configValue;
    Console.WriteLine($"{varName}: ENV={displayEnv}, CONFIG={displayConfig}");
}
Console.WriteLine("=== END AUTH0 DEBUG ===");

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configure JSON serialization to use camelCase
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// Add Auth0 JWT Bearer authentication for API
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}";
    options.Audience = builder.Configuration["Auth0:Audience"];
    options.RequireHttpsMetadata = false; // For development
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Register HttpClient for Ollama API
builder.Services.AddHttpClient<IOllamaService, OllamaService>();
builder.Services.AddScoped<IOllamaService, OllamaService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Set up routing - this needs to be first
app.UseRouting();

// Use authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map controllers BEFORE static files (important!)
// This ensures API routes are handled by controllers instead of static files
app.MapControllers();

// Enable static files for the entire application AFTER routes are mapped
app.UseDefaultFiles(); // This will look for default files like index.html
app.UseStaticFiles();  // This will serve static files from wwwroot

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.RequireAuthorization(); // Require authentication for this endpoint

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
