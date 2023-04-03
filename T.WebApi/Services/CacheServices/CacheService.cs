using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Runtime.Caching;

namespace T.WebApi.Services.CacheServices
{
    public interface ICacheService
    {
        T GetData<T> (string key);
        bool SetData<T> (T value, DateTimeOffset expirationTime, string key = null);
        object RemoveData(string key = null);
    }
    public class CacheService : ICacheService
    {
        private ObjectCache _memoryCache = MemoryCache.Default;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CacheService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetCacheKey() { var request = _httpContextAccessor.HttpContext?.Request; var path = request?.Path.HasValue == true ? request.Path.Value : string.Empty; return $"cache_{path}"; }
        public T GetData<T>(string key)
        {
            var _key = key != null ? key : GetCacheKey();
            try
            {
                T item = (T) _memoryCache.Get(_key);
                return item;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public object RemoveData(string key = null)
        {
            var result = true;

            try
            {
                if (!string.IsNullOrEmpty(GetCacheKey()) || key != null)
                {
                    var res = _memoryCache.Remove(key != null ? key :GetCacheKey());
                }
                else
                {
                    result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool SetData<T>(T value, DateTimeOffset expirationTime, string key = null)
        {
            var result = true;

            try
            {
                if (!string.IsNullOrEmpty(GetCacheKey()) || key != null)
                {
                    _memoryCache.Set(key != null ? key : GetCacheKey(), value, expirationTime);
                }
                else
                {
                    result = false;
                }
                return result;
            }
            catch(Exception ex) 
            {
                throw;
            }
        }
    }
}
