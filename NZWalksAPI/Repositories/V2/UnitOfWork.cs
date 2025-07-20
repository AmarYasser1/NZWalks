
using NZWalks.API.Data;

namespace NZWalks.API.Repositories.V2
{
    public class UnitOfWork : IUnitOfWork
    {
        public IRegionRepository RegionRepository { get; private set; }

        public IWalkRepository WalkRepository { get; private set; }

        private readonly NZWalksDbContext _nZWalksDbContext;

        public UnitOfWork(NZWalksDbContext nZWalksDbContext)
        {
            RegionRepository = new SQLRegionRepository(nZWalksDbContext);
            WalkRepository = new SQLWalkRepository(nZWalksDbContext);
            _nZWalksDbContext = nZWalksDbContext;
        }

        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            await _nZWalksDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
