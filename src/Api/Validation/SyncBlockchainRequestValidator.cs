using FluentValidation;
using ICMarketsTest.Api.Requests;
using ICMarketsTest.Core.Blockchains;

namespace ICMarketsTest.Api.Validation;

public sealed class SyncBlockchainRequestValidator : AbstractValidator<SyncBlockchainRequest>
{
    public SyncBlockchainRequestValidator()
    {
        RuleFor(request => request.Network)
            .NotEmpty()
            .Length(1, 32)
            .Matches("^[a-z0-9-]+$")
            .Must(BeSupportedNetwork)
            .WithMessage("Network must be one of the supported BlockCypher chains.");
    }

    private static bool BeSupportedNetwork(string network)
    {
        return BlockchainsCatalog.TryGet(network, out _);
    }
}
