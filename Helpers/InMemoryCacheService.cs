using Microsoft.Extensions.Caching.Memory;

namespace APITemplate.Services
{
    /// <summary>
    /// Implementacion local de cache en memoria para entornos sin Redis.
    /// </summary>
    public class InMemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        public InMemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var options = new MemoryCacheEntryOptions();

            if (expiration.HasValue)
            {
                options.SetAbsoluteExpiration(expiration.Value);
            }

            _cache.Set(key, value, options);
            return Task.FromResult(true);
        }

        public Task<T?> GetAsync<T>(string key)
        {
            if (_cache.TryGetValue(key, out T? value))
            {
                return Task.FromResult(value);
            }

            return Task.FromResult(default(T));
        }

        public Task<bool> RemoveAsync(string key)
        {
            _cache.Remove(key);
            return Task.FromResult(true);
        }
    }
}
