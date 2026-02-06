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

    [Fact]
    public async Task SyncThenSnapshots_ReturnsStoredItem()
    {
        var syncResponse = await _client.PostAsync("/api/blockchains/sync", JsonContent.Create(new
        {
            network = "btc-main"
        }));
        syncResponse.IsSuccessStatusCode.Should().BeTrue();

        var snapshots = await _client.GetFromJsonAsync<List<BlockchainSnapshotResponse>>(
            "/api/blockchains/snapshots?network=btc-main");

        snapshots.Should().NotBeNull();
        var snapshotList = snapshots ?? throw new InvalidOperationException("Snapshots response is null.");
        snapshotList.Should().Contain(snapshot => snapshot.Network == "btc-main");
    }

    private sealed record BlockchainSnapshotResponse(string Network);
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

public sealed class SnapshotOrderingTests
{
    [Fact]
    public async Task Snapshots_AreOrderedByCreatedAtDescending()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"blockchain-{Guid.NewGuid()}.db");
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite($"Data Source={dbPath};Pooling=False")
            .Options;

        using (var dbContext = new AppDbContext(options))
        {
            dbContext.Database.EnsureCreated();

            dbContext.BlockchainSnapshots.Add(new ICMarketsTest.Infrastructure.Persistence.Entities.BlockchainSnapshot
            {
                Id = Guid.NewGuid(),
                Network = "btc-main",
                SourceUrl = "https://api.blockcypher.com/v1/btc/main",
                Payload = "{}",
                CreatedAt = DateTime.UtcNow.AddMinutes(-5)
            });
            dbContext.BlockchainSnapshots.Add(new ICMarketsTest.Infrastructure.Persistence.Entities.BlockchainSnapshot
            {
                Id = Guid.NewGuid(),
                Network = "btc-main",
                SourceUrl = "https://api.blockcypher.com/v1/btc/main",
                Payload = "{}",
                CreatedAt = DateTime.UtcNow
            });

            await dbContext.SaveChangesAsync();
            dbContext.Database.CloseConnection();
        }

        using (var dbContext = new AppDbContext(options))
        {
            var repo = new ICMarketsTest.Infrastructure.Persistence.Repositories.BlockchainSnapshotRepository(dbContext);
            var result = await repo.GetAsync("btc-main", null, CancellationToken.None);

            result.Should().HaveCount(2);
            result[0].CreatedAt.Should().BeAfter(result[1].CreatedAt);
        }

        File.Delete(dbPath);
    }
}
