using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories.V2
{
    public class SQLRegionRepository : Repository<Region>, IRegionRepository
    {
        public SQLRegionRepository(NZWalksDbContext nZWalksDbContext) : base(nZWalksDbContext) 
        { 
        }
        public async Task<Region?> UpdateAsync(Guid id, Region region, CancellationToken cancellationToken = default)
        {
            var existingRegion = await GetByIdAsync(id, cancellationToken: cancellationToken);

            if (existingRegion is null)
                return null;

            existingRegion.Name = region.Name;
            existingRegion.Code = region.Code;
            existingRegion.RegionImageUrl = region.RegionImageUrl;

            return existingRegion;
        }
    }
}
