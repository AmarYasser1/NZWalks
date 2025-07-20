using System.Threading;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories.V2
{
    public class SQLWalkRepository : Repository<Walk>, IWalkRepository
    {
        public SQLWalkRepository(NZWalksDbContext nZWalksDbContext) : base(nZWalksDbContext)
        {
        }

        public async Task<IEnumerable<Walk>> GetSpecialAllAsync(WalkQueryParameters walkQueryParameters, 
            CancellationToken cancellationToken, bool tracked = true)
        {
            var walks = _nZWalksDbContext.Walks
                          .Include(w => w.Difficulty)
                          .Include(w => w.Region).AsQueryable();

            walks = tracked ? walks : walks.AsNoTracking();

            walks = Filter(walkQueryParameters, walks);
            walks = Sort(walkQueryParameters, walks);
            walks = Paginate(walkQueryParameters, walks);

            return await walks.ToListAsync(cancellationToken);
        }

        public async Task<Walk?> UpdateAsync(Guid id,Walk walk, CancellationToken cancellationToken = default)
        {
            var existWalk = await GetByIdAsync(id, cancellationToken: cancellationToken);

            if (existWalk is null)
                return null;

            existWalk.Name = walk.Name;
            existWalk.Description = walk.Description;
            existWalk.WalkImageUrl = walk.WalkImageUrl;
            existWalk.LengthInKm = walk.LengthInKm;
            existWalk.RegionId = walk.RegionId;
            existWalk.DifficultyId = walk.DifficultyId;

            return existWalk;
        }


        private IQueryable<Walk> Filter(WalkQueryParameters walkQueryParameters, IQueryable<Walk> walks)
        {
            if (!string.IsNullOrEmpty(walkQueryParameters.FilterOn) && !string.IsNullOrEmpty(walkQueryParameters.FilterQuery))
            {
                if (walkQueryParameters.FilterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
                    walks = walks.Where(w => w.Name.Contains(walkQueryParameters.FilterQuery));
            }

            return walks;
        }
        private IQueryable<Walk> Sort(WalkQueryParameters walkQueryParameters, IQueryable<Walk> walks)
        {
            var allowedSortFields = new[] { "Name", "LengthInKm", "RegionId", "DifficultyId" };

            if (!string.IsNullOrEmpty(walkQueryParameters.SortBy)
                && allowedSortFields.Contains(walkQueryParameters.SortBy, StringComparer.OrdinalIgnoreCase))
            {
                walks = walkQueryParameters.IsDescending ?
                    walks.OrderByDescending(w => EF.Property<object>(w, walkQueryParameters.SortBy)) :
                    walks.OrderBy(w => EF.Property<object>(w, walkQueryParameters.SortBy));
            }

            return walks;
        }
        private IQueryable<Walk> Paginate(WalkQueryParameters query, IQueryable<Walk> walks)
        {
            return walks
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize);
        }
    }
}
