using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ICMarketsTest.GatewayTests;

public sealed class GatewaySecurityTests : IClassFixture<GatewayTestFactory>
{
    private readonly HttpClient _client;

    public GatewaySecurityTests(GatewayTestFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task MissingApiKey_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ValidApiKey_AllowsRequest()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/");
        request.Headers.Add("X-Api-Key", "test-key");

        var response = await _client.SendAsync(request);
        response.IsSuccessStatusCode.Should().BeTrue();
    }
}

public sealed class GatewayRateLimitTests : IClassFixture<GatewayTestFactory>
{
    private readonly HttpClient _client;

    public GatewayRateLimitTests(GatewayTestFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task RateLimiter_Returns429_WhenExceeded()
    {
        HttpResponseMessage? lastResponse = null;

        for (var i = 0; i < 25; i++)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/");
            request.Headers.Add("X-Api-Key", "test-key");
            lastResponse = await _client.SendAsync(request);
        }

        lastResponse.Should().NotBeNull();
        lastResponse!.StatusCode.Should().Be(System.Net.HttpStatusCode.TooManyRequests);
    }
}

public sealed class GatewayTestFactory : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(config =>
        {
            var settings = new Dictionary<string, string?>
            {
                ["Gateway:ApiKey"] = "test-key"
            };
            config.AddInMemoryCollection(settings);
        });

        return base.CreateHost(builder);
    }
}
