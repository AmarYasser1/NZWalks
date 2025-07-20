using System.Linq.Expressions;

namespace NZWalks.API.Repositories.V2
{
    public interface IRepository<T> where T : class 
    {
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null,string? includeProperities = null, CancellationToken cancellationToken = default
            , bool tracked = true);
        Task<T?> GetByIdAsync(Guid id,string? includeProperities = null ,CancellationToken cancellationToken = default, bool tracked = true);
        Task<T?> GetAsync(Expression<Func<T, bool>> filter, string? includeProperities = null, CancellationToken cancellationToken = default, bool tracked = true);

        Task AddAsync(T entity, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }
}
