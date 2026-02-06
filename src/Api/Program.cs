using FluentValidation;
using FluentValidation.AspNetCore;
using ICMarketsTest.Api.Middleware;
using ICMarketsTest.Api.Options;
using ICMarketsTest.Api.Validation;
using ICMarketsTest.Contracts;
using ICMarketsTest.Core.Blockchains;
using ICMarketsTest.Core.Events;
using ICMarketsTest.Core.Handlers;
using ICMarketsTest.Core.Interfaces;
using ICMarketsTest.Infrastructure.Clients;
using ICMarketsTest.Infrastructure.Persistence.Data;
using ICMarketsTest.Infrastructure.Persistence.Interfaces;
using ICMarketsTest.Infrastructure.Persistence.Options;
using ICMarketsTest.Infrastructure.Persistence.Repositories;
using ICMarketsTest.Infrastructure.Persistence.Stores;
using ICMarketsTest.Infrastructure.Persistence.UnitOfWork;
using ICMarketsTest.Infrastructure.Events;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<SyncBlockchainRequestValidator>();
builder.Services.AddScoped<IBlockchainSnapshotRepository, BlockchainSnapshotRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ISnapshotStore, EfSnapshotStore>();
var blockCypherMinDelay =
    builder.Configuration.GetValue<int?>("BlockCypher:MinDelayMilliseconds") ?? 350;
var blockCypherTimeoutSeconds =
    builder.Configuration.GetValue<int?>("BlockCypher:TimeoutSeconds") ?? 10;
builder.Services.AddSingleton(new BlockCypherClientOptions
{
    MinDelayMilliseconds = blockCypherMinDelay
});
builder.Services.AddHttpClient<IBlockCypherClient, BlockCypherClient>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(Math.Max(1, blockCypherTimeoutSeconds));
});
var snapshotMinInterval =
    builder.Configuration.GetValue<int?>("Snapshots:MinIntervalSeconds") ?? 30;
builder.Services.AddSingleton(new SnapshotDedupOptions
{
    MinIntervalSeconds = snapshotMinInterval
});
var snapshotMaxLimit =
    builder.Configuration.GetValue<int?>("Snapshots:MaxLimit") ?? 500;
builder.Services.AddSingleton(new SnapshotQueryOptions
{
    MaxLimit = snapshotMaxLimit
});
builder.Services.AddSingleton<IEventPublisher, InMemoryEventPublisher>();
builder.Services.AddScoped<GetSnapshotsHandler>();
builder.Services.AddScoped<SyncBlockchainHandler>();
builder.Services.AddScoped<SyncAllBlockchainsHandler>();
builder.Services.AddAutoMapper(
    typeof(ICMarketsTest.Api.Mapping.ApiMappingProfile).Assembly,
    typeof(ICMarketsTest.Infrastructure.Mapping.InfrastructureMappingProfile).Assembly);
var configuredNetworks = builder.Configuration
    .GetSection("Blockchains:Networks")
    .Get<BlockchainNetworkOptions[]>() ?? Array.Empty<BlockchainNetworkOptions>();
if (configuredNetworks.Length > 0)
{
    var definitions = configuredNetworks
        .Where(network => !string.IsNullOrWhiteSpace(network.Key))
        .Select(network => new BlockchainDefinition(
            network.Key,
            string.IsNullOrWhiteSpace(network.Name) ? network.Key : network.Name,
            network.Url))
        .Where(definition => !string.IsNullOrWhiteSpace(definition.Url))
        .ToList();

    BlockchainsCatalog.ReplaceAll(definitions);
}
var dbFilePath = builder.Configuration["Database:FilePath"] ?? "..\\..\\sql\\blockchain.db";
var dbDirectory = Path.GetDirectoryName(dbFilePath);
if (!string.IsNullOrWhiteSpace(dbDirectory))
{
    Directory.CreateDirectory(dbDirectory);
}
var connectionString = $"Data Source={dbFilePath}";
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCors", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddHealthChecks();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var apiXml = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var apiXmlPath = Path.Combine(AppContext.BaseDirectory, apiXml);
    options.IncludeXmlComments(apiXmlPath, includeControllerXmlComments: true);

    var appAssembly = typeof(BlockchainSnapshotDto).Assembly;
    var appXml = $"{appAssembly.GetName().Name}.xml";
    var appXmlPath = Path.Combine(AppContext.BaseDirectory, appXml);
    options.IncludeXmlComments(appXmlPath);
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILoggerFactory>()
        .CreateLogger("RequestLogging");
    var endpointName = context.GetEndpoint()?.DisplayName ?? "unknown";

    try
    {
        await next();
    }
    finally
    {
        logger.LogInformation("HTTP {Method} {Path} -> {StatusCode} ({Endpoint})",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            endpointName);
    }
});

app.UseMiddleware<ExceptionHandlingMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("DefaultCors");
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();

public partial class Program
{
}
