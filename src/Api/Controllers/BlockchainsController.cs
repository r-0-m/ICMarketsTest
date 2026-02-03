using ICMarketsTest.Api.Requests;
using ICMarketsTest.Application.Blockchains;
using ICMarketsTest.Application.Contracts;
using ICMarketsTest.Application.Interfaces;
using ICMarketsTest.Infrastructure.Stores;
using Microsoft.AspNetCore.Mvc;

namespace ICMarketsTest.Controllers;

/// <summary>
/// BlockCypher network endpoints.
/// </summary>
[ApiController]
[Route("api/blockchains")]
public sealed class BlockchainsController : ControllerBase
{
    private readonly ISnapshotStore _snapshotStore;

    public BlockchainsController()
        : this(new InMemorySnapshotStore())
    {
    }

    public BlockchainsController(ISnapshotStore snapshotStore)
    {
        _snapshotStore = snapshotStore;
    }

    /// <summary>Returns supported BlockCypher networks.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<BlockchainDefinition>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyList<BlockchainDefinition>> GetDefinitions()
    {
        return Ok(BlockchainsCatalog.All);
    }

    /// <summary>Returns snapshot history in descending CreatedAt order.</summary>
    [HttpGet("snapshots")]
    [ProducesResponseType(typeof(IReadOnlyList<BlockchainSnapshotDto>), StatusCodes.Status200OK)]
    public Task<ActionResult<IReadOnlyList<BlockchainSnapshotDto>>> GetSnapshots(
        [FromQuery] GetBlockchainSnapshotsRequest request,
        CancellationToken cancellationToken)
    {
        var network = request.Network;
        if (network is not null && BlockchainsCatalog.TryGet(network, out var definition))
        {
            network = definition.Key;
        }

        return GetSnapshotsInternalAsync(network, request.Limit, cancellationToken);
    }

    /// <summary>Synchronizes a single network and stores a snapshot.</summary>
    [HttpPost("sync")]
    [ProducesResponseType(typeof(BlockchainSnapshotDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public Task<ActionResult<BlockchainSnapshotDto>> SyncBlockchain(
        [FromBody] SyncBlockchainRequest request,
        CancellationToken cancellationToken)
    {
        if (!BlockchainsCatalog.TryGet(request.Network, out var definition))
        {
            return Task.FromResult<ActionResult<BlockchainSnapshotDto>>(
                BadRequest("Unsupported blockchain network."));
        }

        return SyncSingleAsync(definition, cancellationToken);
    }

    /// <summary>Synchronizes all networks and stores snapshots.</summary>
    [HttpPost("sync-all")]
    [ProducesResponseType(typeof(IReadOnlyList<BlockchainSnapshotDto>), StatusCodes.Status200OK)]
    public Task<ActionResult<IReadOnlyList<BlockchainSnapshotDto>>> SyncAll(
        CancellationToken cancellationToken)
    {
        return SyncAllInternalAsync(cancellationToken);
    }

    private async Task<ActionResult<IReadOnlyList<BlockchainSnapshotDto>>> GetSnapshotsInternalAsync(
        string? network,
        int? limit,
        CancellationToken cancellationToken)
    {
        var snapshots = await _snapshotStore.GetAsync(network, limit, cancellationToken);
        return Ok(snapshots);
    }

    private async Task<ActionResult<BlockchainSnapshotDto>> SyncSingleAsync(
        BlockchainDefinition definition,
        CancellationToken cancellationToken)
    {
        var snapshot = new BlockchainSnapshotDto
        {
            Id = Guid.NewGuid(),
            Network = definition.Key,
            SourceUrl = definition.Url,
            Payload = "{\"status\":\"not-implemented\"}",
            CreatedAt = DateTimeOffset.UtcNow
        };
        await _snapshotStore.AddAsync(snapshot, cancellationToken);
        return Ok(snapshot);
    }

    private async Task<ActionResult<IReadOnlyList<BlockchainSnapshotDto>>> SyncAllInternalAsync(
        CancellationToken cancellationToken)
    {
        var snapshots = BlockchainsCatalog.All.Select(definition => new BlockchainSnapshotDto
        {
            Id = Guid.NewGuid(),
            Network = definition.Key,
            SourceUrl = definition.Url,
            Payload = "{\"status\":\"not-implemented\"}",
            CreatedAt = DateTimeOffset.UtcNow
        }).ToList();

        await _snapshotStore.AddRangeAsync(snapshots, cancellationToken);
        return Ok(snapshots);
    }
}
