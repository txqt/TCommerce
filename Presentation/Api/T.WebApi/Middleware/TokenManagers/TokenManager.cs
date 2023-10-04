using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using T.Library.Model.JwtToken;

namespace T.WebApi.Middleware.TokenManagers
{
    public interface ITokenManager
    {
        Task<bool> IsCurrentActiveToken();
        Task DeactivateCurrentAsync();
        Task<bool> IsActiveAsync(string token);
        Task DeactivateAsync(string token);
    }

    public class TokenManager : ITokenManager
    {
        private readonly IMemoryCache _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOptions<JwtOptions> _jwtOptions;

        public TokenManager(IMemoryCache cache,
                IHttpContextAccessor httpContextAccessor,
                IOptions<JwtOptions> jwtOptions
            )
        {
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
            _jwtOptions = jwtOptions;
        }

        public async Task<bool> IsCurrentActiveToken()
            => await IsActiveAsync(GetCurrentAsync());

        public async Task DeactivateCurrentAsync()
            => await DeactivateAsync(GetCurrentAsync());

        public async Task<bool> IsActiveAsync(string token)
            => !_cache.TryGetValue(GetKey(token), out _);

        public async Task DeactivateAsync(string token)
            => await Task.Run(() => _cache.Set(GetKey(token), true,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow =
                        TimeSpan.FromMinutes(_jwtOptions.Value.AccessTokenExpirationInMinutes)
                }));

        private string GetCurrentAsync()
        {
            var authorizationHeader = _httpContextAccessor
                .HttpContext.Request.Headers["authorization"];

            return authorizationHeader == StringValues.Empty
                ? string.Empty
                : authorizationHeader.Single().Split(" ").Last();
        }

        private static string GetKey(string token)
            => $"tokens:{token}:deactivated";
    }
}
