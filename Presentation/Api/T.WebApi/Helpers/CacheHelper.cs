using Microsoft.Extensions.Caching.Memory;

namespace T.WebApi.Helpers
{
    public class CacheHelper
    {
        private readonly IMemoryCache _cache;
        private readonly MemoryCacheEntryOptions _defaultCacheEntryOptions;

        public CacheHelper(IMemoryCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));

            _defaultCacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                SlidingExpiration = TimeSpan.FromMinutes(2),
                Size = 1024,
            };
        }

        public T GetOrCreate<T>(string cacheKey, Func<T> getItemFunction, MemoryCacheEntryOptions cacheEntryOptions = null)
        {
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                throw new ArgumentException("Cache key cannot be null or empty.", nameof(cacheKey));
            }

            if (getItemFunction == null)
            {
                return default(T);
            }

            cacheEntryOptions ??= _defaultCacheEntryOptions;

            if (!_cache.TryGetValue(cacheKey, out T item))
            {
                item = getItemFunction.Invoke();
                _cache.Set(cacheKey, item, cacheEntryOptions);
            }

            // Log here if needed

            return item;
        }


        public void Remove(string cacheKey)
        {
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                throw new ArgumentException("Cache key cannot be null or empty.", nameof(cacheKey));
            }

            _cache.Remove(cacheKey);
        }
    }

}
