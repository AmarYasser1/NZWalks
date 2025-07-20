using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace NZWalks.API.Repositories.V2
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly NZWalksDbContext _nZWalksDbContext;
        private readonly DbSet<T> _dbSet;
        public Repository(NZWalksDbContext nZWalksDbContext)
        {
            _nZWalksDbContext = nZWalksDbContext;
            _dbSet = _nZWalksDbContext.Set<T>();
        }
        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddRangeAsync(entities, cancellationToken);
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter,string? includeProperities = null, CancellationToken cancellationToken = default, bool tracked = true)
        {
            IQueryable<T> query = tracked ? _dbSet : _dbSet.AsNoTracking();

            if (!string.IsNullOrEmpty(includeProperities))
            {
                foreach (var includeProperty in includeProperities
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty.Trim());
                }
            }

            if (filter is not null)
                query = query.Where(filter);

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> filter, string? includeProperities = null, CancellationToken cancellationToken = default, bool tracked = true)
        {
            IQueryable<T> query = tracked ? _dbSet : _dbSet.AsNoTracking();

            if (!string.IsNullOrEmpty(includeProperities))
            {
                foreach (var includeProperty in includeProperities
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty.Trim());
                }
            }

            return await query.FirstOrDefaultAsync(filter, cancellationToken);
        }

        public async Task<T?> GetByIdAsync(Guid id, string? includeProperities = null, CancellationToken cancellationToken = default, bool tracked = true)
        {
            IQueryable<T> query = tracked ? _dbSet : _dbSet.AsNoTracking();

            if (!string.IsNullOrEmpty(includeProperities))
            {
                foreach (var includeProperty in includeProperities
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty.Trim());
                }
            }

            // this is a bad hard code 
            return await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id, cancellationToken);
        }

        public void Remove(T entity)
        {
             _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }
    }
}
