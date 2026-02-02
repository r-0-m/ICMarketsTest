using System.ComponentModel.DataAnnotations;
using ICMarketsTest.Application.Blockchains;
using Microsoft.AspNetCore.Mvc;

namespace ICMarketsTest.Api.Requests;

public sealed class GetBlockchainSnapshotsRequest : IValidatableObject
{
    [FromQuery(Name = "network")]
    public string? Network { get; init; }

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
