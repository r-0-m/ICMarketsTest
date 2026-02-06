using AutoMapper;
using ICMarketsTest.Api.Requests;
using ICMarketsTest.Contracts;
using ICMarketsTest.Core.Blockchains;
using ICMarketsTest.Core.Commands;
using ICMarketsTest.Core.Handlers;
using ICMarketsTest.Core.Queries;
using Microsoft.AspNetCore.Mvc;

namespace ICMarketsTest.Controllers;

/// <summary>
/// BlockCypher network endpoints.
/// </summary>
[ApiController]
[Route("api/blockchains")]
public sealed class BlockchainsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly GetSnapshotsHandler _getSnapshotsHandler;
    private readonly SyncBlockchainHandler _syncBlockchainHandler;
    private readonly SyncAllBlockchainsHandler _syncAllBlockchainsHandler;

    public BlockchainsController(
        IMapper mapper,
        GetSnapshotsHandler getSnapshotsHandler,
        SyncBlockchainHandler syncBlockchainHandler,
        SyncAllBlockchainsHandler syncAllBlockchainsHandler)
    {
        _mapper = mapper;
        _getSnapshotsHandler = getSnapshotsHandler;
        _syncBlockchainHandler = syncBlockchainHandler;
        _syncAllBlockchainsHandler = syncAllBlockchainsHandler;
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

        return GetSnapshotsInternalAsync(request, network, cancellationToken);
    }

    /// <summary>Synchronizes a single network and stores a snapshot.</summary>
    [HttpPost("sync")]
    [ProducesResponseType(typeof(BlockchainSnapshotDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BlockchainSnapshotDto>> SyncBlockchain(
        [FromBody] SyncBlockchainRequest request,
        CancellationToken cancellationToken)
    {
        if (!BlockchainsCatalog.TryGet(request.Network, out var definition))
        {
            return BadRequest("Unsupported blockchain network.");
        }

        var mappedRequest = new SyncBlockchainRequest { Network = definition.Key };
        var command = _mapper.Map<SyncBlockchainCommand>(mappedRequest);
        var snapshot = await _syncBlockchainHandler.HandleAsync(command, cancellationToken);
        return Ok(snapshot);
    }

    /// <summary>Synchronizes all networks and stores snapshots.</summary>
    [HttpPost("sync-all")]
    [ProducesResponseType(typeof(IReadOnlyList<BlockchainSnapshotDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<BlockchainSnapshotDto>>> SyncAll(
        CancellationToken cancellationToken)
    {
        var snapshots = await _syncAllBlockchainsHandler.HandleAsync(
            new SyncAllBlockchainsCommand(),
            cancellationToken);
        return Ok(snapshots);
    }

    private async Task<ActionResult<IReadOnlyList<BlockchainSnapshotDto>>> GetSnapshotsInternalAsync(
        GetBlockchainSnapshotsRequest request,
        string? network,
        CancellationToken cancellationToken)
    {
        var mappedRequest = new GetBlockchainSnapshotsRequest
        {
            Network = network,
            Limit = request.Limit
        };
        var query = _mapper.Map<GetSnapshotsQuery>(mappedRequest);
        var snapshots = await _getSnapshotsHandler.HandleAsync(query, cancellationToken);
        return Ok(snapshots);
    }
}
