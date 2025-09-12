using Microsoft.EntityFrameworkCore;
using Serilog;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using System.Diagnostics;
using Serilog.Enrichers.Span;
using AuditLogging;

// ---------- Serilog ----------
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithSpan()
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {TraceId} {SpanId} {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

// ---------- Builder ----------
var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// ---------- Services ----------
const string serviceName = "orchestration-api";
var activitySource = new ActivitySource(serviceName);

builder.Services.AddSingleton(activitySource);

builder.Services.AddDbContext<DemoDb>((sp, opt) =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("db"))
        .AddInterceptors(sp.GetRequiredService<AuditEfInterceptor>()));

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddSource(serviceName)
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddEntityFrameworkCoreInstrumentation(o => o.SetDbStatementForText = true));

builder.Services.AddAuditLogging(options =>
{
    options.LogEntityFrameworkCore = true;
});

// single typed HttpClient
builder.Services.AddHttpClient("downstream");

builder.Services.AddControllers();

// ---------- App ----------
var app = builder.Build();

app.UseMiddleware<CorrelationMiddleware>();
app.UseMiddleware<AuditMiddleware>();
app.UseSerilogRequestLogging();

app.MapControllers();

app.Run();