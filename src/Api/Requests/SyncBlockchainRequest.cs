using System.ComponentModel.DataAnnotations;

namespace ICMarketsTest.Api.Requests;

public sealed class SyncBlockchainRequest
{
    /// <summary>Network key to synchronize.</summary>
    /// <example>btc-main</example>
    [Required]
    [StringLength(32)]
    [RegularExpression("^[a-z0-9-]+$")]
    public string Network { get; init; } = string.Empty;
}
