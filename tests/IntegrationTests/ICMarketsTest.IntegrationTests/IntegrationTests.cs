using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

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
