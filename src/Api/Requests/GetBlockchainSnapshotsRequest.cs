using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ICMarketsTest.Api.Requests;

public sealed class GetBlockchainSnapshotsRequest
{
    /// <summary>Optional network key filter.</summary>
    /// <example>btc-main</example>
    [FromQuery(Name = "network")]
    [StringLength(32)]
    [RegularExpression("^[a-z0-9-]+$")]
    public string? Network { get; init; }

    /// <summary>Maximum number of snapshots to return.</summary>
    /// <example>50</example>
    [FromQuery(Name = "limit")]
    public int? Limit { get; init; }
}
