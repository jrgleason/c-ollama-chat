using ChatApp.Config;
using ChatApp.Services;
using ChatApp.Auth0;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Expand environment variables in configuration
EnvironmentVariableExpander.ExpandEnvironmentVariables(builder.Configuration);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

// Add authentication options
// Option 1: Use the original security configuration
builder.Services.AddSecurityServices(builder.Configuration);

// Option 2: Use Auth0 authentication (commented out by default)
/*
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie()
.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["Auth0:Domain"] ?? "";
    options.ClientId = builder.Configuration["Auth0:ClientId"] ?? "";
    options.ClientSecret = builder.Configuration["Auth0:ClientSecret"] ?? "";
});
*/

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
app.UseSecurityConfig();

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
