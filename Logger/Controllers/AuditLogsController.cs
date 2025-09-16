using Microsoft.AspNetCore.Mvc;
using AuditLogging;
using System.Threading.Tasks;

namespace Logger.Controllers;

[ApiController]
[Route("[controller]")]
public class AuditLogsController : ControllerBase
{
    private readonly IAuditLogReader _auditLogReader;

    public AuditLogsController(IAuditLogReader auditLogReader)
    {
        _auditLogReader = auditLogReader;
    }

    [HttpGet("{correlationId}")]
    public async Task<IActionResult> GetByCorrelationId(string correlationId)
    {
        var audits = await _auditLogReader.GetByCorrelationIdAsync(correlationId);
        return Ok(audits);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllGroupedByCorrelationId()
    {
        var audits = await _auditLogReader.GetAllGroupedByCorrelationIdAsync();
        return Ok(audits);
    }
}
