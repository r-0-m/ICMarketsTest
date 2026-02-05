namespace ICMarketsTest.Core.Queries;

public sealed record GetSnapshotsQuery(string? Network, int? Limit);
