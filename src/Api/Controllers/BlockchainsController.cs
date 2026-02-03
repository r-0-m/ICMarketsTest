using ICMarketsTest.Api.Requests;
using ICMarketsTest.Application.Blockchains;
using ICMarketsTest.Application.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace ICMarketsTest.Controllers;

/// <summary>
/// BlockCypher network endpoints.
/// </summary>
[ApiController]
[Route("api/blockchains")]
public sealed class BlockchainsController : ControllerBase
{
    /// <summary>Returns supported BlockCypher networks.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<BlockchainDefinition>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyList<BlockchainDefinition>> GetDefinitions()
    {
        return Ok();
    }

    /// <summary>Returns snapshot history in descending CreatedAt order.</summary>
    [HttpGet("snapshots")]
    [ProducesResponseType(typeof(IReadOnlyList<BlockchainSnapshotDto>), StatusCodes.Status200OK)]
    public Task<ActionResult<IReadOnlyList<BlockchainSnapshotDto>>> GetSnapshots(
        [FromQuery] GetBlockchainSnapshotsRequest request,
        CancellationToken cancellationToken)
    {
        _ = request;
        _ = cancellationToken;
        return Task.FromResult<ActionResult<IReadOnlyList<BlockchainSnapshotDto>>>(Ok());
    }

    /// <summary>Synchronizes a single network and stores a snapshot.</summary>
    [HttpPost("sync")]
    [ProducesResponseType(typeof(BlockchainSnapshotDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public Task<ActionResult<BlockchainSnapshotDto>> SyncBlockchain(
        [FromBody] SyncBlockchainRequest request,
        CancellationToken cancellationToken)
    {
        _ = request;
        _ = cancellationToken;
        return Task.FromResult<ActionResult<BlockchainSnapshotDto>>(Ok());
    }

    /// <summary>Synchronizes all networks and stores snapshots.</summary>
    [HttpPost("sync-all")]
    [ProducesResponseType(typeof(IReadOnlyList<BlockchainSnapshotDto>), StatusCodes.Status200OK)]
    public Task<ActionResult<IReadOnlyList<BlockchainSnapshotDto>>> SyncAll(
        CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        return Task.FromResult<ActionResult<IReadOnlyList<BlockchainSnapshotDto>>>(Ok());
    }
}
