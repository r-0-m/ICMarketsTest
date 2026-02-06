using AutoMapper;
using FluentAssertions;
using ICMarketsTest.Api.Requests;
using ICMarketsTest.Core.Events;
using ICMarketsTest.Core.Handlers;
using ICMarketsTest.Core.Interfaces;
using ICMarketsTest.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ICMarketsTest.UnitTests;

public sealed class BlockchainsControllerTests
{
    private static BlockchainsController CreateController()
    {
        var mapperConfig = new MapperConfiguration(cfg =>
            cfg.AddProfile(new ICMarketsTest.Api.Mapping.ApiMappingProfile()));
        var mapper = mapperConfig.CreateMapper();

        var store = new Mock<ISnapshotStore>();
        store.Setup(s => s.GetAsync(It.IsAny<string?>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<ICMarketsTest.Contracts.BlockchainSnapshotDto>());
        store.Setup(s => s.AddAsync(It.IsAny<ICMarketsTest.Contracts.BlockchainSnapshotDto>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        store.Setup(s => s.AddRangeAsync(It.IsAny<IEnumerable<ICMarketsTest.Contracts.BlockchainSnapshotDto>>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var client = new Mock<IBlockCypherClient>();
        client.Setup(c => c.GetBlockchainAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("{\"status\":\"ok\"}");

        var publisher = new Mock<IEventPublisher>();
        publisher.Setup(p => p.PublishAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var getHandler = new GetSnapshotsHandler(store.Object);
        var syncHandler = new SyncBlockchainHandler(client.Object, store.Object, publisher.Object);
        var syncAllHandler = new SyncAllBlockchainsHandler(client.Object, store.Object, publisher.Object);

        return new BlockchainsController(mapper, getHandler, syncHandler, syncAllHandler);
    }

    [Fact]
    public void GetDefinitions_ReturnsOk()
    {
        var controller = CreateController();
        var result = controller.GetDefinitions();

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetSnapshots_ReturnsOk()
    {
        var controller = CreateController();
        var request = new GetBlockchainSnapshotsRequest();

        var result = await controller.GetSnapshots(request, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task SyncBlockchain_ReturnsOk()
    {
        var controller = CreateController();
        var request = new SyncBlockchainRequest { Network = "btc-main" };

        var result = await controller.SyncBlockchain(request, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task SyncAll_ReturnsOk()
    {
        var controller = CreateController();

        var result = await controller.SyncAll(CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }
}

public sealed class SyncBlockchainHandlerTests
{
    [Fact]
    public async Task HandleAsync_StoresSnapshot_AndPublishesEvent()
    {
        var store = new Mock<ISnapshotStore>();
        store.Setup(s => s.AddAsync(It.IsAny<ICMarketsTest.Contracts.BlockchainSnapshotDto>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var client = new Mock<IBlockCypherClient>();
        client.Setup(c => c.GetBlockchainAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("{\"status\":\"ok\"}");

        var publisher = new Mock<IEventPublisher>();
        publisher.Setup(p => p.PublishAsync(It.IsAny<BlockchainSnapshotStored>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new SyncBlockchainHandler(client.Object, store.Object, publisher.Object);

        var result = await handler.HandleAsync(
            new ICMarketsTest.Core.Commands.SyncBlockchainCommand("btc-main"),
            CancellationToken.None);

        result.Network.Should().Be("btc-main");
        store.Verify(s => s.AddAsync(It.IsAny<ICMarketsTest.Contracts.BlockchainSnapshotDto>(),
            It.IsAny<CancellationToken>()), Times.Once);
        publisher.Verify(p => p.PublishAsync(It.IsAny<BlockchainSnapshotStored>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
