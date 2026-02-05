using ICMarketsTest.Contracts;

namespace ICMarketsTest.Core.Events;

/// <summary>
/// Event emitted when a snapshot is stored.
/// </summary>
public sealed record BlockchainSnapshotStored(BlockchainSnapshotDto Snapshot);
