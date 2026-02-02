using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ICMarketsTest.FunctionalTests;

public sealed class HealthAndSnapshotsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public HealthAndSnapshotsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_ReturnsOk()
    {
        var response = await _client.GetAsync("/health");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Snapshots_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/blockchains/snapshots");

        response.IsSuccessStatusCode.Should().BeTrue();
    }
}
