var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddCors(options =>
{
    // later adjust to prefferred scenario
    options.AddPolicy("GatewayCors", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddHealthChecks();

var app = builder.Build();

var gatekeeper = new SemaphoreSlim(1, 1);
var windowStart = DateTime.UtcNow;
var requestCount = 0;
const int permitLimit = 20;
var window = TimeSpan.FromSeconds(10);

app.Use(async (context, next) =>
{
    await gatekeeper.WaitAsync(context.RequestAborted);
    try
    {
        var now = DateTime.UtcNow;
        if (now - windowStart >= window)
        {
            windowStart = now;
            requestCount = 0;
        }

        if (requestCount >= permitLimit)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsync("Rate limit exceeded.");
            return;
        }

        requestCount++;
    }
    finally
    {
        gatekeeper.Release();
    }

    var requiredKey = app.Configuration["Gateway:ApiKey"];
    if (!string.IsNullOrWhiteSpace(requiredKey))
    {
        if (!context.Request.Headers.TryGetValue("X-Api-Key", out var providedKey) ||
            !string.Equals(providedKey.ToString(), requiredKey, StringComparison.Ordinal))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Missing or invalid API key.");
            return;
        }
    }

    await next();
});

app.UseCors("GatewayCors");
app.MapGet("/", () => Results.Ok("Gateway is running."));
app.MapHealthChecks("/health");
app.MapReverseProxy();

app.Run();

public partial class Program
{
}
