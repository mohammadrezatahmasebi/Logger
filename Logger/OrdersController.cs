using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly ILogger<OrderController> _log;
    private readonly DemoDb _db;
    private readonly IHttpClientFactory _http;

    public OrdersController(ILogger<OrderController> logger,
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


        _log.LogInformation("Starting orchestration for order {OrderId}", orderId);

        // 1. save locally
     
            _db.Orders.Add(new Order(orderId, amount));
            await _db.SaveChangesAsync();
        

        // 2. call fake downstream service
   
            var cl = _http.CreateClient("downstream");
            var rsp = await cl.PostAsync("https://httpbin.org/status/404", null);
            _log.LogInformation("Payment service returned {Status}", rsp.StatusCode);
        

        _log.LogInformation("Orchestration finished");
        return Ok(new { orderId });
    }
}

