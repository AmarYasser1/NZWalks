using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories.V2
{
    public interface IRegionRepository : IRepository<Region>
    {
        Task<Region?> UpdateAsync(Guid id, Region region, CancellationToken cancellationToken = default);
    }
}
