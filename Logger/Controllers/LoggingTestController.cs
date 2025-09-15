using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Logger.Controllers;

[ApiController]
[Route("[controller]")]
public class LoggingTestController : ControllerBase
{
    private readonly ILogger<LoggingTestController> _logger;

    public LoggingTestController(ILogger<LoggingTestController> logger)
    {
        _logger = logger;
    }

    [HttpGet("/test-logging")]
    public IActionResult TestLogging()
    {
        _logger.LogInformation("This is a test log message from the LoggingTestController.");
        return Ok("Logged a test message. Check the console output.");
    }
}
