using ICMarketsTest.Contracts;
using ICMarketsTest.Core.Events;
using ICMarketsTest.Core.Handlers;
using ICMarketsTest.Core.Interfaces;
using ICMarketsTest.Infrastructure.Clients;
using ICMarketsTest.Infrastructure.Data;
using ICMarketsTest.Infrastructure.Interfaces;
using ICMarketsTest.Infrastructure.Repositories;
using ICMarketsTest.Infrastructure.Stores;
using ICMarketsTest.Infrastructure.UnitOfWork;
using ICMarketsTest.Infrastructure.Events;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddScoped<IBlockchainSnapshotRepository, BlockchainSnapshotRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ISnapshotStore, EfSnapshotStore>();
var blockCypherMinDelay =
    builder.Configuration.GetValue<int?>("BlockCypher:MinDelayMilliseconds") ?? 350;
builder.Services.AddSingleton(new BlockCypherClientOptions
{
    MinDelayMilliseconds = blockCypherMinDelay
});
builder.Services.AddHttpClient<IBlockCypherClient, BlockCypherClient>();
builder.Services.AddSingleton<IEventPublisher, InMemoryEventPublisher>();
builder.Services.AddScoped<GetSnapshotsHandler>();
builder.Services.AddScoped<SyncBlockchainHandler>();
builder.Services.AddScoped<SyncAllBlockchainsHandler>();
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
