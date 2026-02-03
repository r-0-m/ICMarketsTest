using FluentAssertions;
using ICMarketsTest.Api.Requests;
using ICMarketsTest.Application.Interfaces;
using ICMarketsTest.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ICMarketsTest.UnitTests;

public sealed class BlockchainsControllerTests
{
    private static BlockchainsController CreateController()
    {
        var store = new Mock<ISnapshotStore>();
        store.Setup(s => s.GetAsync(It.IsAny<string?>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<ICMarketsTest.Application.Contracts.BlockchainSnapshotDto>());
        store.Setup(s => s.AddAsync(It.IsAny<ICMarketsTest.Application.Contracts.BlockchainSnapshotDto>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        store.Setup(s => s.AddRangeAsync(It.IsAny<IEnumerable<ICMarketsTest.Application.Contracts.BlockchainSnapshotDto>>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var client = new Mock<IBlockCypherClient>();
        client.Setup(c => c.GetBlockchainAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("{\"status\":\"ok\"}");

        return new BlockchainsController(store.Object, client.Object);
    }

    [Fact]
    public void GetDefinitions_ReturnsOk()
    {
        var controller = CreateController();
        var result = controller.GetDefinitions();

        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task GetSnapshots_ReturnsOk()
    {
        var controller = CreateController();
        var request = new GetBlockchainSnapshotsRequest();

        var result = await controller.GetSnapshots(request, CancellationToken.None);

        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task SyncBlockchain_ReturnsOk()
    {
        var controller = CreateController();
        var request = new SyncBlockchainRequest { Network = "btc-main" };

        var result = await controller.SyncBlockchain(request, CancellationToken.None);

        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task SyncAll_ReturnsOk()
    {
        var controller = CreateController();

        var result = await controller.SyncAll(CancellationToken.None);

        result.Should().BeOfType<OkResult>();
    }
}
