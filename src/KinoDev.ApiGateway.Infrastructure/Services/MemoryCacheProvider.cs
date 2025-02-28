using Microsoft.Extensions.Caching.Memory;

namespace KinoDev.ApiGateway.Infrastructure.Services
{
    public interface ICacheProvider
    {
        T? Get<T>(string key);
        void Set<T>(string key, T item, TimeSpan expiration);
    }

    public class MemoryCacheProvider : ICacheProvider
    {
        private readonly IMemoryCache _cache;

        public MemoryCacheProvider(IMemoryCache cache)
        {
            _cache = cache;
        }

        public T? Get<T>(string key)
        {
            _cache.TryGetValue(key, out T value);
            return value;
        }

        public void Set<T>(string key, T item, TimeSpan expiration)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            };
            _cache.Set(key, item, options);
        }
    }
}
