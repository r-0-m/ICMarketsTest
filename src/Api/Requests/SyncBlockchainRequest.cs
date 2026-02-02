using System.ComponentModel.DataAnnotations;
using ICMarketsTest.Application.Blockchains;

namespace ICMarketsTest.Api.Requests;

public sealed class SyncBlockchainRequest : IValidatableObject
{
    [Required]
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
