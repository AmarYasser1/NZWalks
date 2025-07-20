using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories.V2
{
    public interface IWalkRepository : IRepository<Walk>
    {
        Task<IEnumerable<Walk>> GetSpecialAllAsync(WalkQueryParameters walkQueryParameters, 
            CancellationToken cancellationToken = default, bool tracked = true);
        Task<Walk?> UpdateAsync(Guid id, Walk walk, CancellationToken cancellationToken = default);
    }
}
