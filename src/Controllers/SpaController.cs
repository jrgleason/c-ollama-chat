using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers;

[ApiController]
public class SpaController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    
    public SpaController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }    [HttpGet]
    [Route("/")]
    [Route("/{*catchAll:regex(^(?!api|assets).*)}")] // Exclude /api and /assets paths
    public IActionResult Index()
    {
        // Serve the SPA's index.html for the root and any non-API route
        // This allows client-side routing to work
        return PhysicalFile(Path.Combine(_environment.WebRootPath, "index.html"), "text/html");
    }
}
