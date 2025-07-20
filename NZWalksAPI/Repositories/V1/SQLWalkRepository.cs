using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories.V1
{
    public class SQLWalkRepository : IWalkRepository
    {
        private readonly NZWalksDbContext _context;
        public SQLWalkRepository(NZWalksDbContext context)
        {
            _context = context;
        }

        public async Task<Walk> CreateAsync(Walk walk)
        {
            await _context.Walks.AddAsync(walk);
            await _context.SaveChangesAsync();

            // This is the most important thing in this project(Reload the navigation prop)
            await _context.Entry(walk).Reference(w => w.Region).LoadAsync();
            await _context.Entry(walk).Reference(w => w.Difficulty).LoadAsync();

            return walk;
        }

        public async Task<Walk?> DeleteAsync(Guid id)
        {
            var deletedWalkModel = await GetByIdAsync(id);

            if (deletedWalkModel is null)
                return null;

            _context.Walks.Remove(deletedWalkModel);
            await _context.SaveChangesAsync();

            return deletedWalkModel;
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

        public async Task<IEnumerable<Walk>> GetAllAsync(WalkQueryParameters walkQueryParameters)
        {
            var walks = _context.Walks
                          .Include(w => w.Difficulty)
                          .Include(w => w.Region).AsQueryable();

            walks = Filter(walkQueryParameters, walks);
            walks = Sort(walkQueryParameters, walks);
            walks = Paginate(walkQueryParameters, walks);

            return await walks.ToListAsync();
        }

        public async Task<Walk?> GetByIdAsync(Guid id)
        {
            return await _context.Walks
                .Include(w => w.Difficulty)
                .Include(w => w.Region)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<Walk?> UpdateAsync(Guid id, Walk walk)
        {
            var existWalk = await GetByIdAsync(id);

            if (existWalk is null)
                return null;

            existWalk.Name = walk.Name;
            existWalk.Description = walk.Description;
            existWalk.WalkImageUrl = walk.WalkImageUrl;
            existWalk.LengthInKm = walk.LengthInKm;
            existWalk.RegionId = walk.RegionId;
            existWalk.DifficultyId = walk.DifficultyId;

            await _context.SaveChangesAsync();

            return existWalk;
        }
    }
}
