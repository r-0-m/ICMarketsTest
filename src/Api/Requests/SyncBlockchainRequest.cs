using System.ComponentModel.DataAnnotations;
using ICMarketsTest.Application.Blockchains;

namespace ICMarketsTest.Api.Requests;

public sealed class SyncBlockchainRequest : IValidatableObject
{
    /// <summary>Network key to synchronize.</summary>
    /// <example>btc-main</example>
    [Required]
    [StringLength(32)]
    [RegularExpression("^[a-z0-9-]+$")]
    public string Network { get; init; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!BlockchainsCatalog.TryGet(Network, out _))
        {
            yield return new ValidationResult(
                "Network must be one of the supported BlockCypher chains.",
                new[] { nameof(Network) });
        }
    }
}
