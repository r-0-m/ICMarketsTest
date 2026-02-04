using System.ComponentModel.DataAnnotations;
using ICMarketsTest.Core.Blockchains;
using Microsoft.AspNetCore.Mvc;

namespace ICMarketsTest.Api.Requests;

public sealed class GetBlockchainSnapshotsRequest : IValidatableObject
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
    [Range(1, 500)]
    public int? Limit { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Network is not null && !BlockchainsCatalog.TryGet(Network, out _))
        {
            yield return new ValidationResult(
                "Network must be one of the supported BlockCypher chains.",
                new[] { nameof(Network) });
        }
    }
}
