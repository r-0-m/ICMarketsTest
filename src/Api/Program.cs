using ICMarketsTest.Application.Interfaces;
using ICMarketsTest.Infrastructure.Clients;
using ICMarketsTest.Infrastructure.Stores;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddSingleton<ISnapshotStore, InMemorySnapshotStore>();
var blockCypherMinDelay =
    builder.Configuration.GetValue<int?>("BlockCypher:MinDelayMilliseconds") ?? 350;
builder.Services.AddSingleton(new BlockCypherClientOptions
{
    MinDelayMilliseconds = blockCypherMinDelay
});
builder.Services.AddHttpClient<IBlockCypherClient, BlockCypherClient>();

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

    var appAssembly = typeof(ICMarketsTest.Application.Contracts.BlockchainSnapshotDto).Assembly;
    var appXml = $"{appAssembly.GetName().Name}.xml";
    var appXmlPath = Path.Combine(AppContext.BaseDirectory, appXml);
    options.IncludeXmlComments(appXmlPath);
});

var app = builder.Build();

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
