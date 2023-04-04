
//using Microsoft.AspNetCore.Components.Authorization;
//using Microsoft.AspNetCore.Http;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http.Headers;
//using System.Security.Claims;
//using System.Text;
//using System.Text.Json;
//using System.Threading.Tasks;

//namespace T.Library.Model.Common
//{
//    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
//    {
//        private readonly HttpClient _httpClient;
//        private readonly IHttpContextAccessor _localStorage;

//        public ApiAuthenticationStateProvider(HttpClient httpClient, ILocalStorageService localStorage)
//        {
//            _httpClient = httpClient;
//            _localStorage = localStorage;
//        }
//        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
//        {
//            var savedToken = await _localStorage.GetItemAsync<string>("authToken");

//            if (string.IsNullOrWhiteSpace(savedToken))
//            {
//                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
//            }

//            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);

//            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(savedToken), "jwt")));
//        }

//        public void MarkUserAsAuthenticated(string token)
//        {
//            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));
//            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
//            NotifyAuthenticationStateChanged(authState);
//        }

//        public void MarkUserAsLoggedOut()
//        {
//            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
//            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
//            NotifyAuthenticationStateChanged(authState);
//        }

//        public IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
//        {
//            var claims = new List<Claim>();
//            var payload = jwt.Split('.')[1];
//            var jsonBytes = ParseBase64WithoutPadding(payload);
//            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

//            keyValuePairs.TryGetValue(ClaimTypes.Role, out object roles);

//            if (roles != null)
//            {
//                if (roles.ToString().Trim().StartsWith("["))
//                {
//                    var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());

//                    foreach (var parsedRole in parsedRoles)
//                    {
//                        claims.Add(new Claim(ClaimTypes.Role, parsedRole));
//                    }
//                }
//                else
//                {
//                    claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
//                }

//                keyValuePairs.Remove(ClaimTypes.Role);
//            }

//            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));

//            return claims;
//        }

//        private byte[] ParseBase64WithoutPadding(string base64)
//        {
//            base64 = ValidateBase64EncodedString(base64);
//            return Convert.FromBase64String(base64);
//        }

//        private string ValidateBase64EncodedString(string inputText)
//        {
//            string stringToValidate = inputText;
//            stringToValidate = stringToValidate.Replace('-', '+'); // 62nd char of encoding
//            stringToValidate = stringToValidate.Replace('_', '/'); // 63rd char of encoding
//            switch (stringToValidate.Length % 4) // Pad with trailing '='s
//            {
//                case 0: break; // No pad chars in this case
//                case 2: stringToValidate += "=="; break; // Two pad chars
//                case 3: stringToValidate += "="; break; // One pad char
//                default:
//                    throw new System.Exception(
//             "Illegal base64url string!");
//            }

//            return stringToValidate;
//        }
//    }
//}
