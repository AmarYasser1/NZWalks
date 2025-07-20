using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace NZWalks.API.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _redisCache;

        public CacheService(IDistributedCache redisCache)
        {
            _redisCache = redisCache;
        }

        public async Task<T?> GetDataAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            var data = await _redisCache.GetStringAsync(key, cancellationToken);

            return data is null ? default : JsonSerializer.Deserialize<T>(data);
        }
        public async Task SetDataAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5),
            };

            await _redisCache.SetStringAsync(key, JsonSerializer.Serialize(value) , options, cancellationToken);
        }

        public async Task RemoveDataAsync(string key, CancellationToken cancellationToken = default)
        {
            await _redisCache.RemoveAsync(key, cancellationToken);
        }
    }
}
