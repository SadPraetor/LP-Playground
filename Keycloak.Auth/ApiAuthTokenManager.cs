using System.Text.Json.Serialization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json.Linq;

namespace Keycloak.Auth
{
    public class ApiAuthTokenManager
    {      
        private JwtSecurityToken? _accessToken;
        private bool _accessTokenExpired => DateTime.UtcNow > _accessToken?.ValidTo;
       
        private JwtSecurityToken? _refreshToken;
        
        private JwtSecurityTokenHandler _jwtHandler = new JwtSecurityTokenHandler();

        private readonly TokenApiClient _tokenApiClient;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<ApiAuthTokenManager> _logger;


        public ApiAuthTokenManager(
            TokenApiClient tokenApiClient, 
            IHttpContextAccessor contextAccessor,
            ILogger<ApiAuthTokenManager> logger)
        {
            _tokenApiClient = tokenApiClient;
            _contextAccessor = contextAccessor;
            _logger = logger;
        }

        public async Task SetValuesAsync(HttpContext currentContext)
        {
            var accessToken = await currentContext.GetTokenAsync("access_token");
            var refreshToken = await currentContext.GetTokenAsync("refresh_token");
            FillTokens(accessToken, refreshToken);
        }

        private void FillTokens (string? accessToken, string? refreshToken)
        {
            if(accessToken is null || refreshToken is null)
            {
                _logger.LogWarning("Unable to fill token information, one of the tokens is null");
                return;
            }

            _accessToken = _jwtHandler.ReadJwtToken(accessToken);
            _refreshToken = _jwtHandler.ReadJwtToken(refreshToken);
        }


        public async Task<string> GetTokenAsync()
        {
            _ = _accessToken ?? throw new InvalidOperationException("Access token is null");

            if (!_accessTokenExpired)
            {
                return _accessToken.ToString();
            }

            _ = _refreshToken ?? throw new InvalidOperationException("Refresh token is null");
            var data = await _tokenApiClient.RefreshTokenAsync(_refreshToken!.ToString());
            FillTokens(data.AccessToken, data.RefreshToken);
           
            await RefreshCookieAsync();

            return _accessToken.ToString();
        }


        private async Task RefreshCookieAsync()
        {
            var context = _contextAccessor.HttpContext;
            var authResult = await context.AuthenticateAsync("Cookies");
            if (authResult?.Principal != null)
            {
                var authProperties = authResult.Properties;

                // Get the existing tokens, update them with the new values
                var tokens = authProperties.GetTokens().ToList();

                // Replace or add the access token
                tokens.RemoveAll(t => t.Name == "access_token");
                tokens.Add(new AuthenticationToken { Name = "access_token", Value = _accessToken.ToString() });

                // Replace or add the refresh token
                tokens.RemoveAll(t => t.Name == "refresh_token");
                tokens.Add(new AuthenticationToken { Name = "refresh_token", Value = _refreshToken.ToString() });

              
                authProperties.StoreTokens(tokens);

                // Re-issue the cookie with updated tokens
                await context.SignInAsync("Cookies", authResult.Principal, authProperties);

                _logger.LogInformation("Cookie refreshed");
            }
        }


        
    }
}
