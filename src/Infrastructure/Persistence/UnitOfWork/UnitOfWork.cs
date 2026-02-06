using ICMarketsTest.Infrastructure.Persistence.Data;
using ICMarketsTest.Infrastructure.Persistence.Interfaces;

namespace ICMarketsTest.Infrastructure.Persistence.UnitOfWork;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _dbContext;
    private readonly IBlockchainSnapshotRepository _snapshots;

    public UnitOfWork(AppDbContext dbContext, IBlockchainSnapshotRepository snapshots)
    {
        _dbContext = dbContext;
        _snapshots = snapshots;
    }

    public IBlockchainSnapshotRepository Snapshots => _snapshots;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
