using System.Net.Http.Json;
using FluentAssertions;
using ICMarketsTest.Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

namespace ICMarketsTest.IntegrationTests;

public sealed class BlockchainsApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public BlockchainsApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetDefinitions_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/blockchains");

        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task SyncBlockchain_ReturnsOk()
    {
        var response = await _client.PostAsync("/api/blockchains/sync", JsonContent.Create(new
        {
            network = "btc-main"
        }));

        response.IsSuccessStatusCode.Should().BeTrue();
    }
}

public sealed class DatabaseInitializationTests
{
    [Fact]
    public void EnsureCreated_CreatesSqliteFile()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"blockchain-{Guid.NewGuid()}.db");
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite($"Data Source={dbPath};Pooling=False")
            .Options;

        using (var dbContext = new AppDbContext(options))
        {
            dbContext.Database.EnsureCreated();
            dbContext.Database.CloseConnection();
        }

        File.Exists(dbPath).Should().BeTrue();
        File.Delete(dbPath);
    }
}
