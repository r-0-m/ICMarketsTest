using ICMarketsTest.Api.Requests;
using Microsoft.AspNetCore.Mvc;

namespace ICMarketsTest.Controllers;

[ApiController]
[Route("api/blockchains")]
public sealed class BlockchainsController : ControllerBase
{
    [HttpGet]
    public ActionResult GetDefinitions()
    {
        return Ok();
    }

    [HttpGet("snapshots")]
    public Task<ActionResult> GetSnapshots(
        [FromQuery] GetBlockchainSnapshotsRequest request,
        CancellationToken cancellationToken)
    {
        _ = request;
        _ = cancellationToken;
        return Task.FromResult<ActionResult>(Ok());
    }

    [HttpPost("sync")]
    public Task<ActionResult> SyncBlockchain(
        [FromBody] SyncBlockchainRequest request,
        CancellationToken cancellationToken)
    {
        _ = request;
        _ = cancellationToken;
        return Task.FromResult<ActionResult>(Ok());
    }

    [HttpPost("sync-all")]
    public Task<ActionResult> SyncAll(
        CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        return Task.FromResult<ActionResult>(Ok());
    }
}
