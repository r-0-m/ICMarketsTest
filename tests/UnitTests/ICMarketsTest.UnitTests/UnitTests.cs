using FluentAssertions;
using ICMarketsTest.Api.Requests;
using ICMarketsTest.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace ICMarketsTest.UnitTests;

public sealed class BlockchainsControllerTests
{
    [Fact]
    public void GetDefinitions_ReturnsOk()
    {
        var controller = new BlockchainsController();
        var result = controller.GetDefinitions();

        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task GetSnapshots_ReturnsOk()
    {
        var controller = new BlockchainsController();
        var request = new GetBlockchainSnapshotsRequest();

        var result = await controller.GetSnapshots(request, CancellationToken.None);

        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task SyncBlockchain_ReturnsOk()
    {
        var controller = new BlockchainsController();
        var request = new SyncBlockchainRequest { Network = "btc-main" };

        var result = await controller.SyncBlockchain(request, CancellationToken.None);

        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task SyncAll_ReturnsOk()
    {
        var controller = new BlockchainsController();

        var result = await controller.SyncAll(CancellationToken.None);

        result.Should().BeOfType<OkResult>();
    }
}
