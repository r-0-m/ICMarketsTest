using FluentValidation;
using ICMarketsTest.Api.Options;
using ICMarketsTest.Api.Requests;
using ICMarketsTest.Core.Blockchains;

namespace ICMarketsTest.Api.Validation;

public sealed class GetBlockchainSnapshotsRequestValidator : AbstractValidator<GetBlockchainSnapshotsRequest>
{
    public GetBlockchainSnapshotsRequestValidator(SnapshotQueryOptions options)
    {
        var maxLimit = options.MaxLimit > 0 ? options.MaxLimit : 500;

        RuleFor(request => request.Network)
            .Matches("^[a-z0-9-]+$")
            .When(request => !string.IsNullOrWhiteSpace(request.Network))
            .Must(BeSupportedNetwork)
            .When(request => !string.IsNullOrWhiteSpace(request.Network))
            .WithMessage("Network must be one of the supported BlockCypher chains.");

        RuleFor(request => request.Limit)
            .InclusiveBetween(1, maxLimit)
            .When(request => request.Limit.HasValue);
    }

    private static bool BeSupportedNetwork(string? network)
    {
        return network is not null && BlockchainsCatalog.TryGet(network, out _);
    }
}
