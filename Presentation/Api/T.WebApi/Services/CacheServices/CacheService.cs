using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using System.Runtime.Caching;

namespace T.WebApi.Services.CacheServices
{
    public interface ICacheService
    {
        T Get<T>(string key);
        void Set<T>(string key, T value, TimeSpan? expiry = null);
        void Remove(string key);
    }

    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public T Get<T>(string key)
        {
            return _cache.Get<T>(key);
        }

        public void Set<T>(string key, T value, TimeSpan? expiry = null)
        {
            if (expiry.HasValue)
            {
                _cache.Set(key, value, expiry.Value);
            }
            else
            {
                _cache.Set(key, value);
            }
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}
