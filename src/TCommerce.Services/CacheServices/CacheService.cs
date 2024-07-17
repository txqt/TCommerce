using System.Runtime.Caching;
using TCommerce.Core.Interface;

namespace TCommerce.Services.CacheServices
{
    public class CacheService : ICacheService
    {
        ObjectCache _memoryCache = MemoryCache.Default;
        public T GetData<T>(string key)
        {
            try
            {
                T item = (T)_memoryCache.Get(key);
                return item;
            }
            catch 
            {
                throw;
            }
        }
        public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            bool res = true;
            try
            {
                if (!string.IsNullOrEmpty(key))
                {
                    _memoryCache.Set(key, value, expirationTime);
                }
            }
            catch 
            {
                throw;
            }
            return res;
        }
        public object RemoveData(string key)
        {
            try
            {
                if (!string.IsNullOrEmpty(key))
                {
                    return _memoryCache.Remove(key);
                }
            }
            catch 
            {
                throw;
            }
            return false;
        }
    }
}
