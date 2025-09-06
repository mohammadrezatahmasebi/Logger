using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/order")]
public class OrderController : ControllerBase
{
    private static readonly ActivitySource _src = new("orchestration-api");
    private readonly ILogger<OrderController> _log;
    private readonly DemoDb _db;
    private readonly IHttpClientFactory _http;

    public OrderController(ILogger<OrderController> logger,
        DemoDb db,
        IHttpClientFactory http)
    {
        _log = logger;
        _db = db;
        _http = http;
    }

    [HttpPost]
    public async Task<IActionResult> Create(decimal amount)
    {
        var orderId = Guid.NewGuid();
        using var main = _src.StartActivity("OrchestrateOrder");
        main?.SetTag("order.id", orderId);

        _log.LogInformation("Starting orchestration for order {OrderId}", orderId);

        // 1. save locally
        using (_src.StartActivity("SaveOrder"))
        {
            _db.Orders.Add(new Order(orderId, amount));
            await _db.SaveChangesAsync();
        }

        // 2. call fake downstream service
        using (_src.StartActivity("CallPayment"))
        {
            var cl = _http.CreateClient("downstream");
            var rsp = await cl.PostAsync("https://httpbin.org/status/404", null);
            _log.LogInformation("Payment service returned {Status}", rsp.StatusCode);
        }

        _log.LogInformation("Orchestration finished");
        return Ok(new { orderId });
    }
}

