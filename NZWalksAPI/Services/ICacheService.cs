namespace NZWalks.API.Services
{
    public interface ICacheService
    {
        Task<T?> GetDataAsync<T>(string key, CancellationToken cancellationToken = default);
        Task SetDataAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
        Task RemoveDataAsync(string key, CancellationToken cancellationToken = default);
    }
}
