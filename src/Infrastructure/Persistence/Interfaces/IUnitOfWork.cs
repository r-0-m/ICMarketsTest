namespace ICMarketsTest.Infrastructure.Persistence.Interfaces;

/// <summary>
/// Unit of work for persistence operations.
/// </summary>
public interface IUnitOfWork
{
    IBlockchainSnapshotRepository Snapshots { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
